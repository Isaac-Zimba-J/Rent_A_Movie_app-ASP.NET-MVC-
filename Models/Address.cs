using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

[Table("Address")]
[Index("Street", IsUnique = true)]
[Index("Zipcode", IsUnique = true)]
public partial class Address
{
    [Key]
    [Column("ZIPCODE")]
    public long Zipcode { get; set; }

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    [InverseProperty("ZipCodeNavigation")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
