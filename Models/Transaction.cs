using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Transaction")]
public partial class Transaction
{
    [Key]
    [Column("TID")]
    public long Tid { get; set; }

    public long? Customer { get; set; }

    public long? Movie { get; set; }

    public string DateRented { get; set; } = null!;

    //monthrented
    public long MonthRented { 
        get {
                DateTime monthRented; 
                if(DateTime.TryParse(DateRented, out monthRented)){
                    return monthRented.Month;
                }
                else{
                    return 0;
                }
                
            }
     }

    [ForeignKey("Customer")]
    [InverseProperty("Transactions")]
    public virtual Customer? CustomerNavigation { get; set; }

    [ForeignKey("Movie")]
    [InverseProperty("Transactions")]
    public virtual Movie? MovieNavigation { get; set; }
}
