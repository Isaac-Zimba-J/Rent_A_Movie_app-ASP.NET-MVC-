using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RAM_APP.Models;

public partial class RentAmovieDbContext : DbContext
{
    public RentAmovieDbContext()
    {
    }

    public RentAmovieDbContext(DbContextOptions<RentAmovieDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Starring> Starrings { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Name=ConnectionStrings:DBConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.Property(e => e.Zipcode).ValueGeneratedNever();
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(e => e.Rating).HasDefaultValueSql("'No Rating'");
            entity.Property(e => e.Tax).HasDefaultValueSql("10");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(t => t.CustomerNavigation).WithMany(j => j.Transactions);

            entity.HasOne(t => t.MovieNavigation).WithMany(j => j.Transactions).OnDelete(DeleteBehavior.Cascade); 
        }
        );

        modelBuilder.Entity<Starring>(entity =>
        {
            entity.HasOne(d => d.ActorNavigation).WithMany(p => p.Starrings).OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.MovieNavigation).WithMany(p => p.Starrings).OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
