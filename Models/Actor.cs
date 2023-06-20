using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Actor")]
[Index("Aid", IsUnique = true)]
public partial class Actor
{
    [Key]
    [Column("AID")]
    public long Aid { get; set; }

    public string Name { get; set; } = null!;

    [InverseProperty("ActorNavigation")]
    public virtual ICollection<Starring> Starrings { get; set; } = new List<Starring>();
}
