using BlazorServerCPP.Data;
using BlazorServerCPP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BlazorServerCPP.Endpoints;

public static class LicenceEndpoints
{
    public static void MapLicenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/licence").AllowAnonymous();

        group.MapGet("/state", async (string? licenceId, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(licenceId))
                return Results.BadRequest(new ErrorResponse("INVALID_REQUEST", "licenceId mancante"));

            var licenza = await db.Licenze.AsNoTracking()
                .FirstOrDefaultAsync(l => l.IdLicenza == licenceId);

            if (licenza is null)
                return Results.NotFound(new ErrorResponse("LICENCE_NOT_FOUND", $"Licenza {licenceId} non trovata"));

            var now = DateTime.UtcNow;
            var scadenzaUtc = licenza.DataScadenza.HasValue
                ? new DateTime(licenza.DataScadenza.Value.Year, licenza.DataScadenza.Value.Month, licenza.DataScadenza.Value.Day,
                               23, 59, 59, DateTimeKind.Utc)
                : (DateTime?)null;

            return Results.Ok(new LicenceStateResponse(
                Id: licenza.IdLicenza,
                Scadenza: scadenzaUtc,
                Pagato: licenza.Pagato ?? false,
                Scaduta: scadenzaUtc.HasValue && scadenzaUtc.Value < now,
                ServerTime: now));
        });

        group.MapPost("/activate", async (ActivateLicenceRequest req, AppDbContext db, HttpContext http) =>
        {
            var now = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(req.ClientId) || string.IsNullOrWhiteSpace(req.NumeroCommessa))
                return Results.BadRequest(new ErrorResponse("INVALID_REQUEST", "Dati mancanti"));

            var cliente = await db.Clienti.FindAsync(req.ClientId);
            if (cliente is null)
                return Results.NotFound(new ErrorResponse("CLIENT_NOT_FOUND", "Cliente non trovato"));

            var esistente = await db.Licenze
                .AnyAsync(l => l.IdCliente == req.ClientId && l.DataScadenza > DateOnly.FromDateTime(now));

            if (esistente)
                return Results.Conflict(new ErrorResponse("LICENCE_EXISTS", "Licenza già attiva"));

            //controllo che la licenza sia scaduta ma sia anche pagata
            var nonPagateScadute = await db.Licenze
                .FirstOrDefaultAsync(l => l.IdCliente == req.ClientId &&
                                          l.DataScadenza < DateOnly.FromDateTime(now) &&
                                          l.Pagato == true);
            if (nonPagateScadute == null)
                return Results.BadRequest(new ErrorResponse("INVALID_REQUEST", "La licenza non è stata pagata")); 

            var idLicenza = $"{req.NumeroCommessa}{now}{req.ClientId}";

            var licenza = new Licenze
            {
                IdLicenza = idLicenza,
                IdCliente = req.ClientId,
                IdImpianto = req.ImpiantoId, 
                DataAttivazione = DateOnly.FromDateTime(now),
                DataScadenza = DateOnly.FromDateTime(now.AddYears(1)),
                Pagato = false
            };

            db.Licenze.Add(licenza);
            await db.SaveChangesAsync();

            var nuova = new Licenza(
                Id: licenza.IdLicenza,
                Scadenza: licenza.DataScadenza.HasValue
                    ? new DateTime(
                        licenza.DataScadenza.Value.Year,
                        licenza.DataScadenza.Value.Month,
                        licenza.DataScadenza.Value.Day,
                        23, 59, 59,
                        DateTimeKind.Utc)
                    : DateTime.MinValue,
                Pagato: licenza.Pagato ?? false); 

            http.Response.Headers["X-Licence-Id"] = idLicenza;
            http.Response.Headers["X-Licence-Scadenza"] = nuova.Scadenza.ToString();
            http.Response.Headers["X-Licence-Pagato"] = nuova.Pagato.ToString();

            return Results.Ok(nuova);
        });

        group.MapPost("/new", async (GetNewLicenceRequest req, AppDbContext db) =>
        {
            var cliente = await db.Clienti.FindAsync(req.ClientId);
            if (cliente is null)
                return Results.NotFound(new ErrorResponse("CLIENT_NOT_FOUND", "Cliente non trovato"));

            var ultimaLicenza = await db.Licenze
                .Where(l => l.IdCliente == req.ClientId)
                .OrderByDescending(l => l.DataScadenza)
                .FirstOrDefaultAsync();

            if (ultimaLicenza != null && ultimaLicenza.Pagato == false)
                return Results.StatusCode(402);

            var now = DateTime.UtcNow;

            var idLicenza = $"{req.Commessa}{DateOnly.FromDateTime(now)}{req.ClientId}";

            var nuova = new Licenze
            {
                IdLicenza = idLicenza,
                IdCliente = req.ClientId,
                //sbagliato ma temporaneo
                IdImpianto = req.ImpiantoId, 
                DataAttivazione = DateOnly.FromDateTime(now),
                DataScadenza = DateOnly.FromDateTime(now.AddYears(1)),
                Pagato = false
            };

            db.Licenze.Add(nuova);
            await db.SaveChangesAsync();

            // 🔥 genera file finto
            var contenuto = System.Text.Encoding.UTF8.GetBytes($"LICENCE:{idLicenza}");

            var result = Results.File(
                contenuto,
                contentType: "application/octet-stream",
                fileDownloadName: $"{idLicenza}.lic",
                lastModified: now,
                entityTag: null,
                enableRangeProcessing: false
            );

            return result;
        }); 
    }

    private record LicenceStateResponse(
        string Id,
        DateTime? Scadenza,
        bool Pagato,
        bool Scaduta,   
        DateTime ServerTime);

    private record ErrorResponse(string Code, string Message);

    private record ActivateLicenceRequest(
        string ClientId,
        int ImpiantoId, 
        string NumeroCommessa);

    private record GetNewLicenceRequest(
        string ClientId,
        int ImpiantoId,
        string Commessa);

    private record Licenza(string Id, DateTime Scadenza, bool Pagato);
}
