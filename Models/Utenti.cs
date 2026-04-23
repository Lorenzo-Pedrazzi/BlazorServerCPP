using System;
using System.Collections.Generic;

namespace BlazorServerCPP.Models;

public partial class Utenti
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Ruolo { get; set; } = null!;

    public DateTime DataCreazione { get; set; }

    public DateTime? DataUltimoLogin { get; set; }
}
