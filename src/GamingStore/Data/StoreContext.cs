using System;
using System.Collections.Generic;
using System.Text.Json;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace GamingStore.Data
{
    public class StoreContext : IdentityDbContext<Customer, IdentityRole, string>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Relationships

            #region OneToOne

            // Order 1-1 Payment
            //modelBuilder.Entity<Order>().HasOne(b => b.Payment).WithOne(i => i.Order)
            //    .HasForeignKey<Payment>(p => p.OrderForeignKey);

            #endregion

            #region ManyToMany

            // OrderItem: Order 1..x-1..x Item
            modelBuilder.Entity<OrderItem>().HasKey(orderItem => new {orderItem.OrderId, orderItem.ItemId});
            modelBuilder.Entity<OrderItem>().HasOne(orderItem => orderItem.Order).WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId);
            modelBuilder.Entity<OrderItem>().HasOne(orderItem => orderItem.Item).WithMany(item => item.OrderItems)
                .HasForeignKey(orderItem => orderItem.ItemId);

            // StoreItem: Store 1..x-1..x Item
            modelBuilder.Entity<StoreItem>().HasKey(storeItem => new {storeItem.StoreId, storeItem.ItemId});
            modelBuilder.Entity<StoreItem>().HasOne(storeItem => storeItem.Store).WithMany(store => store.StoreItems)
                .HasForeignKey(storeItem => storeItem.StoreId);
            modelBuilder.Entity<StoreItem>().HasOne(storeItem => storeItem.Item).WithMany(item => item.StoreItems)
                .HasForeignKey(storeItem => storeItem.ItemId);

            modelBuilder.HasSequence<int>("Order_seq", schema: "dbo")
                .StartsAt(0)
                .IncrementsBy(1);

            modelBuilder.Entity<Customer>()
                .Property(o => o.CustomerNumber)
                .HasDefaultValueSql("NEXT VALUE FOR dbo.Order_seq");

            modelBuilder.Entity<RelatedItem>().HasKey(table => new {
                CustomerId = table.CustomerNumber,
                ItemId = table.ItemId
            });
            #endregion

            #endregion

            #region ObjectConverationHandling

            modelBuilder.Entity<Item>().Property(i => i.PropertiesList).HasConversion(
                v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            modelBuilder.Entity<Store>().Property(s => s.Address).HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Address>(v));
            modelBuilder.Entity<Customer>().Property(c => c.Address).HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Address>(v));
            modelBuilder.Entity<Store>().Property(s => s.OpeningHours).HasConversion(
                v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<OpeningHours>>(v));
            modelBuilder.Entity<Order>().Property(c => c.ShippingAddress)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<Address>(v));

            #endregion

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Store>().ToTable("Stores");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Cart>().ToTable("Carts");
        }

        public DbSet<RelatedItem> RelatedItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}