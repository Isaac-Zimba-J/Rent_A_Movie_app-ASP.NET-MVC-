using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Customer")]
[Index("Cid", IsUnique = true)]
public partial class Customer
{
    [Key]
    [Column("CID")]
    public long Cid { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Surname { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public long? ZipCode { get; set; }

    [InverseProperty("CustomerNavigation")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    [ForeignKey("ZipCode")]
    [InverseProperty("Customers")]
    public virtual Address? ZipCodeNavigation { get; set; }
}
