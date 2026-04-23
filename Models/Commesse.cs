using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerCPP.Models;

public partial class Commesse
{
    [Key]
    [Column("id_commessa")]
    public string IdCommessa { get; set; } = null!;

    public virtual ICollection<Impianti> Impiantis { get; set; } = new List<Impianti>();
}
