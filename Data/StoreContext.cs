using System.Collections.Generic;
using GamingStore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GamingStore.Data
{
    public class StoreContext : DbContext
    {
            public StoreContext(DbContextOptions<StoreContext> options) : base(options)
            {
            }

            public DbSet<Customer> Customers{ get; set; }
            public DbSet<Item> Items { get; set; }
            public DbSet<Store> Stores { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Customer>().ToTable("Customers");
                modelBuilder.Entity<Item>().ToTable("Items");
                modelBuilder.Entity<Store>().ToTable("Stores");
                modelBuilder.Entity<Store>().Property(b => b.ItemsList).HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Item, ushort>>(v));
            }
    }
}