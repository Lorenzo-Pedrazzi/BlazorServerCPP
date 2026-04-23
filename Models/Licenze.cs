using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerCPP.Models;

public partial class Licenze
{
    [Key]
    [Column("id_licenza")]
    public string IdLicenza { get; set; } = null!;

    [Column("id_impianto")]
    public int? IdImpianto { get; set; }

    [Column("id_cliente")]
    public string? IdCliente { get; set; }
    [Column("data_attivazione")]
    public DateOnly? DataAttivazione { get; set; }
    [Column("data_scadenza")]
    public DateOnly? DataScadenza { get; set; }
    [Column("pagato")]
    public bool? Pagato { get; set; }

    public virtual Clienti? Cliente { get; set; }

    public virtual Impianti? Impianto { get; set; }
}
