using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerCPP.Models;

public partial class Clienti
{
    [Key]
    [Column("id_cliente")]
    public string IdCliente { get; set; } = null!;
    [Column("nome")]
    public string Nome { get; set; } = null!;
    [Column("iva")]
    public string Iva { get; set; } = null!;

    public virtual ICollection<Impianti> Impiantis { get; set; } = new List<Impianti>();

    public virtual ICollection<Licenze> Licenzes { get; set; } = new List<Licenze>();
}
