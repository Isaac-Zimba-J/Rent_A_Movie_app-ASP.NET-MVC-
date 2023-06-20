using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Starring")]
[Index("Actor","Movie", IsUnique = true)]
public partial class Starring
{
    [Key]
    [Column("SID")]
    public long Sid { get; set; }

    public long? Actor { get; set; }

    public long? Movie { get; set; }

    [ForeignKey("Actor")]
    [InverseProperty("Starrings")]
    public virtual Actor? ActorNavigation { get; set; }

    [ForeignKey("Movie")]
    [InverseProperty("Starrings")]
    public virtual Movie? MovieNavigation { get; set; }
}
