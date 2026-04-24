using BlazorServerCPP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
    }

    private record LicenceStateResponse(
        string Id,
        DateTime? Scadenza,
        bool Pagato,
        bool Scaduta,
        DateTime ServerTime);

    private record ErrorResponse(string Code, string Message);
}
