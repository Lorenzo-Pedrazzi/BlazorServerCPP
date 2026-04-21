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
        var group = app.MapGroup("/auth").DisableAntiforgery();

        group.MapPost("/register", async (
            HttpContext ctx,
            AppDbContext db,
            IPasswordHasher<User> hasher,
            [FromForm] string username,
            [FromForm] string password) =>
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Results.Redirect("/register?error=missing");

            if (password.Length < 6)
                return Results.Redirect("/register?error=short");

            if (await db.Users.AnyAsync(u => u.Username == username))
                return Results.Redirect("/register?error=exists");

            var user = new User { Username = username };
            user.PasswordHash = hasher.HashPassword(user, password);
            db.Users.Add(user);
            await db.SaveChangesAsync();

            await SignIn(ctx, user);
            return Results.Redirect("/");
        });

        group.MapPost("/login", async (
            HttpContext ctx,
            AppDbContext db,
            IPasswordHasher<User> hasher,
            [FromForm] string username,
            [FromForm] string password) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user is null)
                return Results.Redirect("/login?error=invalid");

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                return Results.Redirect("/login?error=invalid");

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = hasher.HashPassword(user, password);
                await db.SaveChangesAsync();
            }

            await SignIn(ctx, user);
            return Results.Redirect("/");
        });

        group.MapPost("/logout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        });
    }

    private static Task SignIn(HttpContext ctx, User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return ctx.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}
