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
            modelBuilder.Entity<Order>()
                .HasOne(b => b.Payment)
                .WithOne(i => i.Order)
                .HasForeignKey<Payment>(p => p.OrderForeignKey);
            #endregion
            #region ManyToMany
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

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new DictionaryJsonConverter());
            //todo: fix serialization of dictionaries to <object's key,value>

            #region DictionariesHandling
            modelBuilder.Entity<Item>()
                .Property(i => i.PropertiesList)
                .HasConversion(
                    v => JsonConvert.SerializeObject(
                        v,
                        Formatting.Indented
                        //, jsonSerializerSettings
                    ),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));

            //modelBuilder.Entity<Order>()
            //    .Property(o => o.Items)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(
            //            v,
            //            Formatting.Indented
            //            //, jsonSerializerSettings
            //            ),
            //        v => JsonConvert.DeserializeObject<Dictionary<Item, uint>>(v));
            //modelBuilder.Entity<Store>()
            //    .Property(s => s.Stock)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(
            //            v,
            //            Formatting.Indented
            //            //, jsonSerializerSettings
            //            ),
            //        v => JsonConvert.DeserializeObject<Dictionary<Item, uint>>(v));
            #endregion
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Store>().ToTable("Stores");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Payment>().ToTable("Payments");

            //    modelBuilder.Entity<Store>().Property(b => b.ItemsList).HasConversion(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<Dictionary<Item, ushort>>(v));
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }



    }
}