using System.Collections.Generic;
using System.Text.Json;
using GamingStore.Contracts;
using GamingStore.Contracts.Converters;
using GamingStore.Models;
using GamingStore.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GamingStore.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Relationships
            #region OneToOne
            // Order 1-1 Payment
            modelBuilder.Entity<Order>()
                .HasOne(b => b.Payment)
                .WithOne(i => i.Order)
                .HasForeignKey<Payment>(p => p.OrderForeignKey);
            #endregion
            #region ManyToMany
            // OrderItem: Order 1..x-1..x Item
            modelBuilder.Entity<OrderItem>()
                .HasKey(orderItem => new { orderItem.OrderId, orderItem.ItemId});
            modelBuilder.Entity<OrderItem>()
                .HasOne(orderItem => orderItem.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId);
            modelBuilder.Entity<OrderItem>()
                .HasOne(orderItem => orderItem.Item)
                .WithMany(item => item.OrderItems)
                .HasForeignKey(orderItem => orderItem.ItemId);

            // StoreItem: Store 1..x-1..x Item
            modelBuilder.Entity<StoreItem>()
                .HasKey(storeItem => new { storeItem.StoreId, storeItem.ItemId });
            modelBuilder.Entity<StoreItem>()
                .HasOne(storeItem => storeItem.Store)
                .WithMany(store => store.StoreItems)
                .HasForeignKey(storeItem => storeItem.StoreId);
            modelBuilder.Entity<StoreItem>()
                .HasOne(storeItem => storeItem.Item)
                .WithMany(item => item.StoreItems)
                .HasForeignKey(storeItem => storeItem.ItemId);
            #endregion
            #endregion
            #region DictionariesHandling
            //var jsonSerializerSettings = new JsonSerializerSettings();
            //jsonSerializerSettings.Converters.Add(new DictionaryJsonConverter());
            modelBuilder.Entity<Item>()
                .Property(i => i.PropertiesList)
                .HasConversion(
                    v => JsonConvert.SerializeObject(
                        v,
                        Formatting.Indented
                        //, jsonSerializerSettings
                    ),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            #endregion

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Store>().ToTable("Stores");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Payment>().ToTable("Payments");
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }



    }
}