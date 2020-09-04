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

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments{ get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //todo: fix serialization of dictionaries to <object's key,value>
            modelBuilder.Entity<Customer>().ToTable("Customers");
            //modelBuilder.Entity<Customer>().HasMany(i => i.OrderHistory).WithOne();
            modelBuilder.Entity<Item>()
                .Property(i=>i.PropertiesList)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            modelBuilder.Entity<Order>()
                .Property(o => o.Items)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Item,uint>>(v));
            modelBuilder.Entity<Store>()
                .Property(s => s.Stock)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Item, uint>>(v));
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Store>().ToTable("Stores");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Payment>().ToTable("Payments");

            //    modelBuilder.Entity<Store>().Property(b => b.ItemsList).HasConversion(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<Dictionary<Item, ushort>>(v));
        }
    }
}