using System.Security.Claims;
using BlazorServerCPP.Data;
using BlazorServerCPP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCPP.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").DisableAntiforgery().AllowAnonymous();

        group.MapPost("/register", async (
            HttpContext ctx,
            AppDbContext db,
            IPasswordHasher<Utenti> hasher,
            [FromForm] string username,
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] string confirmPassword) =>
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
                return Results.Redirect("/register?error=missing");

            if (password.Length < 6)
                return Results.Redirect("/register?error=short");

            if (password != confirmPassword)
                return Results.Redirect("/register?error=mismatch");

            if (await db.Utenti.AnyAsync(u => u.Username == username || u.Email == email))
                return Results.Redirect("/register?error=exists");

            var user = new Utenti
            {
                Username = username,
                Email = email,
                Ruolo = "user",
                DataCreazione = DateTime.UtcNow
            };
            user.PasswordHash = hasher.HashPassword(user, password);
            db.Utenti.Add(user);
            await db.SaveChangesAsync();

            await SignIn(ctx, user);
            return Results.Redirect("/");
        });

        group.MapPost("/login", async (
            HttpContext ctx,
            AppDbContext db,
            IPasswordHasher<Utenti> hasher,
            [FromForm] string username,
            [FromForm] string password,
            [FromForm] string? returnUrl) =>
        {
            var safeReturn = SanitizeReturnUrl(returnUrl);

            var user = await db.Utenti.FirstOrDefaultAsync(u => u.Username == username);
            if (user is null)
                return Results.Redirect($"/login?error=invalid&returnUrl={Uri.EscapeDataString(safeReturn)}");

            PasswordVerificationResult result;
            try
            {
                result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            }
            catch (FormatException)
            {
                // Hash legacy non nel formato di PasswordHasher<T> (es. utenti storici del CPP).
                result = PasswordVerificationResult.Failed;
            }

            if (result == PasswordVerificationResult.Failed)
                return Results.Redirect($"/login?error=invalid&returnUrl={Uri.EscapeDataString(safeReturn)}");

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = hasher.HashPassword(user, password);
            }
            user.DataUltimoLogin = DateTime.UtcNow;
            await db.SaveChangesAsync();

            await SignIn(ctx, user);
            return Results.Redirect(safeReturn);
        });

        group.MapPost("/logout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        });
    }

    private static string SanitizeReturnUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return "/";
        if (!url.StartsWith('/') || url.StartsWith("//")) return "/";
        return url;
    }

    private static Task SignIn(HttpContext ctx, Utenti user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Ruolo ?? "user")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return ctx.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}
