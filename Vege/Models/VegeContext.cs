using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace Vege.Models
{
    public class VegeContext : IdentityDbContext<ApplicationUser>
    {
        ILogger<VegeContext> _logger;
        private IConfigurationRoot _config;

        public VegeContext(DbContextOptions<VegeContext> options, ILogger<VegeContext> logger, IConfigurationRoot config)
          : base(options)
        {
            _logger = logger;
            _config = config;
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer(this._config["ConnectionStrings:VegeConnection"]);
            optionsBuilder.UseMySQL(this._config["ConnectionStrings:VegeConnection"]);
        }
    }
}