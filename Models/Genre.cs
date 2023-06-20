using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Genre")]
[Index("Gid", IsUnique = true)]
[Index("Name", IsUnique = true)]
public partial class Genre
{
    [Key]
    [Column("GID")]
    public long Gid { get; set; }

    public string Name { get; set; } = null!;

    [InverseProperty("GenreNavigation")]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
