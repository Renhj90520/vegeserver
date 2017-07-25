using System;
using System.Data.Entity;

namespace VegeOrderService.Models
{
    public class VegeContext : DbContext
    {
        public VegeContext() : base("VegeDb")
        {

        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Product> Products { get; set; }
        //public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Unit> Units { get; set; }
        //public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}