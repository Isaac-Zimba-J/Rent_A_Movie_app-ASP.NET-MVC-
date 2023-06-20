using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Movie")]
public partial class Movie
{
    [Key]
    [Column("MID")]
    public long Mid { get; set; }

    public long? Genre { get; set; }

    public string Title { get; set; } = null!;

    public string ReleaseDate { get; set; } = null!;

    public string? Rating { get; set; }

    public double Price { get; set; }

    public double Tax { get; set; }

    //Calculate the amount payable from Tax and Price per movie
    public double Amount { 
        get {return Price + ((Price*Tax)/100);}
     }

     public long ReleaseYear { 
        get {
                DateTime releaseDate; 
                if(DateTime.TryParse(ReleaseDate, out releaseDate)){
                    return releaseDate.Year;
                }
                else{
                    return 0;
                }
                
            }
     }

    [ForeignKey("Genre")]
    [InverseProperty("Movies")]
    public virtual Genre? GenreNavigation { get; set; }

    [InverseProperty("MovieNavigation")]
    public virtual ICollection<Starring> Starrings { get; set; } = new List<Starring>();

    [InverseProperty("MovieNavigation")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
