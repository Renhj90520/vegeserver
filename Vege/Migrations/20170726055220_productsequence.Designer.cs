using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Vege.Models;

namespace Vege.Migrations
{
    [DbContext(typeof(VegeContext))]
    [Migration("20170726055220_productsequence")]
    partial class productsequence
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Vege.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Area")
                        .HasMaxLength(20);

                    b.Property<string>("City")
                        .HasMaxLength(20);

                    b.Property<string>("Name")
                        .HasMaxLength(20);

                    b.Property<string>("OpenId")
                        .HasMaxLength(28);

                    b.Property<string>("Phone")
                        .HasMaxLength(15);

                    b.Property<string>("Province")
                        .HasMaxLength(20);

                    b.Property<string>("Street")
                        .HasMaxLength(80);

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Vege.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IconPath")
                        .HasMaxLength(70);

                    b.Property<string>("Name")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Vege.Models.Favorite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OpenId")
                        .HasMaxLength(28);

                    b.Property<int>("ProductId");

                    b.HasKey("Id");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Vege.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<string>("CancelReason")
                        .HasMaxLength(160);

                    b.Property<DateTime>("CancelTime");

                    b.Property<DateTime>("CreateTime");

                    b.Property<double>("DeliveryCharge");

                    b.Property<DateTime>("FinishTime");

                    b.Property<string>("IsPaid")
                        .HasMaxLength(1);

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("NotifyState")
                        .HasMaxLength(1);

                    b.Property<string>("OpenId")
                        .HasMaxLength(28);

                    b.Property<string>("RefundNote")
                        .HasMaxLength(160);

                    b.Property<int>("State");

                    b.Property<string>("WXOrderId")
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Vege.Models.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Count");

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

                    b.Property<string>("Path")
                        .HasMaxLength(70);

                    b.Property<int?>("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("Vege.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .HasMaxLength(20);

                    b.Property<double>("Price");

                    b.Property<int>("Sequence");

                    b.Property<int>("State");

                    b.Property<double>("Step");

                    b.Property<double>("TotalCount");

                    b.Property<int>("UnitId");

                    b.Property<string>("UnitName")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Vege.Models.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(10);

                    b.Property<double>("Step");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("Vege.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .HasMaxLength(20);

                    b.Property<string>("Name")
                        .HasMaxLength(20);

                    b.Property<string>("OpenId")
                        .HasMaxLength(28);

                    b.Property<string>("Password")
                        .HasMaxLength(32);

                    b.Property<string>("Phone")
                        .HasMaxLength(15);

                    b.Property<string>("Province")
                        .HasMaxLength(20);

                    b.Property<int?>("Sex");

                    b.Property<string>("UserName")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("Users");
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
