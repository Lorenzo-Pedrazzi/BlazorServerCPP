using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerCPP.Server.Models;

[Table("utenti")]
public class Utente
{
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Column("email")]
    [Required, MaxLength(255), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Column("password_hash")]
    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("ruolo")]
    [Required, MaxLength(20)]
    public string Ruolo { get; set; } = RuoliUtente.User;

    [Column("data_creazione")]
    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    [Column("data_ultimo_login")]
    public DateTime? DataUltimoLogin { get; set; }
}

public static class RuoliUtente
{
    public const string Admin = "admin";
    public const string User = "user";
    public const string Installatore = "installatore";

    public static readonly string[] All = { Admin, User, Installatore };

    public static bool IsValido(string? ruolo) => ruolo is not null && All.Contains(ruolo);
}
