using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerCPP.Models;

public partial class Impianti
{
    [Key]
    [Column("id_impianto")]
    public int IdImpianto { get; set; }
    [Column("nome")]
    public string? Nome { get; set; }
    [Column("id_cliente")]
    public string? IdCliente { get; set; }

    [Column("id_commessa")]
    public string? IdCommessa { get; set; }

    public virtual Clienti? Cliente { get; set; }

    public virtual Commesse? Commessa { get; set; }

    public virtual ICollection<Licenze> Licenzes { get; set; } = new List<Licenze>();
}
