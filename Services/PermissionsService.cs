using System.Collections.Concurrent;
using System.Globalization;

namespace BlazorServerCPP.Services;

public class PermissionsService
{
    // Lowercase per rispettare il check constraint 'utenti_ruolo_check' su PostgreSQL
    public IReadOnlyList<string> Ruoli { get; } = new[] { "admin", "user", "installatore" };

    public IReadOnlyList<string> Pagine { get; } = new[] { "Dashboard", "Configurazione", "Log" };

    private readonly ConcurrentDictionary<(string Pagina, string Ruolo), bool> _permessi;

    public PermissionsService()
    {
        _permessi = new ConcurrentDictionary<(string, string), bool>
        {
            [("Dashboard",      "admin")]        = true,
            [("Dashboard",      "user")]         = true,
            [("Dashboard",      "installatore")] = true,
            [("Configurazione", "admin")]        = true,
            [("Configurazione", "user")]         = false,
            [("Configurazione", "installatore")] = false,
            [("Log",            "admin")]        = true,
            [("Log",            "user")]         = false,
            [("Log",            "installatore")] = true,
        };
    }

    public bool IsAllowed(string pagina, string ruolo) =>
        _permessi.TryGetValue((pagina, ruolo.ToLowerInvariant()), out var v) && v;

    public void SetPermission(string pagina, string ruolo, bool allowed) =>
        _permessi[(pagina, ruolo.ToLowerInvariant())] = allowed;

    public static string Display(string ruolo) =>
        string.IsNullOrEmpty(ruolo)
            ? ruolo
            : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(ruolo);
}
