using BlazorServerCPP.Server.Data;
using BlazorServerCPP.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCPP.Server.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db) => _db = db;

    public async Task<(bool ok, string? error)> RegisterAsync(string username, string email, string password)
    {
        username = username.Trim();
        email = email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            return (false, "Username troppo corto (min 3 caratteri).");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return (false, "Password troppo corta (min 8 caratteri).");

        if (await _db.Utenti.AnyAsync(u => u.Username == username))
            return (false, "Username già in uso.");
        if (await _db.Utenti.AnyAsync(u => u.Email == email))
            return (false, "Email già registrata.");

        var utente = new Utente
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            Ruolo = RuoliUtente.User,
            DataCreazione = DateTime.UtcNow
        };

        _db.Utenti.Add(utente);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<Utente?> LoginAsync(string usernameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            return null;

        var key = usernameOrEmail.Trim();
        var keyLower = key.ToLowerInvariant();

        var utente = await _db.Utenti
            .FirstOrDefaultAsync(u => u.Username == key || u.Email == keyLower);

        if (utente is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, utente.PasswordHash)) return null;

        utente.DataUltimoLogin = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return utente;
    }
}
