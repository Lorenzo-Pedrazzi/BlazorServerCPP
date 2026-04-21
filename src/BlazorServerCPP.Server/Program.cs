using System.Security.Claims;
using BlazorServerCPP.Server.Components;
using BlazorServerCPP.Server.Data;
using BlazorServerCPP.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/login";
        opt.LogoutPath = "/auth/logout";
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/auth/register", async (
    HttpContext ctx,
    IAuthService auth,
    [FromForm] string username,
    [FromForm] string email,
    [FromForm] string password,
    [FromForm] string confirmPassword) =>
{
    if (password != confirmPassword)
        return Results.Redirect("/register?error=" + Uri.EscapeDataString("Le password non coincidono."));

    var (ok, error) = await auth.RegisterAsync(username, email, password);
    if (!ok)
        return Results.Redirect("/register?error=" + Uri.EscapeDataString(error ?? "Errore."));

    return Results.Redirect("/login?registered=1");
}).DisableAntiforgery();

app.MapPost("/auth/login", async (
    HttpContext ctx,
    IAuthService auth,
    [FromForm] string username,
    [FromForm] string password) =>
{
    var utente = await auth.LoginAsync(username, password);
    if (utente is null)
        return Results.Redirect("/login?error=" + Uri.EscapeDataString("Credenziali non valide."));

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, utente.Id.ToString()),
        new(ClaimTypes.Name, utente.Username),
        new(ClaimTypes.Email, utente.Email),
        new(ClaimTypes.Role, utente.Ruolo)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapPost("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.Run();
