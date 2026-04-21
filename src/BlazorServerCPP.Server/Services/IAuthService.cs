using BlazorServerCPP.Server.Models;

namespace BlazorServerCPP.Server.Services;

public interface IAuthService
{
    Task<(bool ok, string? error)> RegisterAsync(string username, string email, string password);
    Task<Utente?> LoginAsync(string usernameOrEmail, string password);
}
