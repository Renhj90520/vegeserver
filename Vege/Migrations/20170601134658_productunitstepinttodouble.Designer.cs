using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Vege.Models;

namespace Vege.Migrations
{
    [DbContext(typeof(VegeContext))]
    [Migration("20170601134658_productunitstepinttodouble")]
    partial class productunitstepinttodouble
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Vege.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Area");

                    b.Property<string>("City");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("Province");

                    b.Property<string>("Street");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Vege.Models.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<int>("ProductId");

                    b.Property<int?>("ShoppingCartId");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingCartId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Vege.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IconPath");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Vege.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<string>("CancelReason");

                    b.Property<DateTime>("CancelTime");

                    b.Property<DateTime>("CreateTime");

                    b.Property<DateTime>("FinishTime");

                    b.Property<string>("OpenId");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Vege.Models.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<int?>("OrderId");

                    b.Property<double>("Price");

                    b.Property<int>("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("Vege.Models.Picture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Path");

                    b.Property<int?>("ProductId");

                    b.Property<int>("isPrimary");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("Vege.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<int>("State");

                    b.Property<double>("Step");

                    b.Property<double>("TotalCount");

                    b.Property<int>("UnitId");

                    b.Property<string>("UnitName");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Vege.Models.ShoppingCart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OpenId");

                    b.HasKey("Id");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("Vege.Models.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<double>("Step");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("Vege.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("OpenId");

                    b.Property<string>("Password");

                    b.Property<string>("Phone");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Vege.Models.CartItem", b =>
                {
                    b.HasOne("Vege.Models.ShoppingCart")
                        .WithMany("Products")
                        .HasForeignKey("ShoppingCartId");
                });

            modelBuilder.Entity("Vege.Models.OrderItem", b =>
                {
                    b.HasOne("Vege.Models.Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId");
                });

            modelBuilder.Entity("Vege.Models.Picture", b =>
                {
                    b.HasOne("Vege.Models.Product")
                        .WithMany("Pictures")
                        .HasForeignKey("ProductId");
                });
        }
    }
}
