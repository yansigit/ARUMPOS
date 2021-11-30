﻿// <auto-generated />
using System;
using ArumModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ArumModels.Migrations
{
    [DbContext(typeof(ArumDbContext))]
    [Migration("20211006020552_Add_Order_Columns")]
    partial class Add_Order_Columns
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("ArumModels.Models.CarouselImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .HasColumnType("TinyText");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("CarouselImage");
                });

            modelBuilder.Entity("ArumModels.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ArumModels.Models.Coupon", b =>
                {
                    b.Property<string>("CouponCode")
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("CouponCode");

                    b.HasIndex("ShopId");

                    b.ToTable("Coupon");
                });

            modelBuilder.Entity("ArumModels.Models.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Ingredient");
                });

            modelBuilder.Entity("ArumModels.Models.Menu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BackgroundColor")
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("ColdPrice")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("HotPrice")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Menu");
                });

            modelBuilder.Entity("ArumModels.Models.Option", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Option");
                });

            modelBuilder.Entity("ArumModels.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CardName")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CardNumber")
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DiscountPrice")
                        .HasColumnType("int");

                    b.Property<DateTime>("EstimatedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Message")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PaymentCode")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("ShopId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Tid")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.HasIndex("UserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("ArumModels.Models.OrderedMenu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BackgroundColor")
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderedMenu");
                });

            modelBuilder.Entity("ArumModels.Models.OrderedOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Body")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<int?>("OrderedMenuId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("OrderedMenuId");

                    b.ToTable("OrderedOption");
                });

            modelBuilder.Entity("ArumModels.Models.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("TinyText");

                    b.Property<int>("BaudRate")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsOpened")
                        .HasColumnType("tinyint(1)");

                    b.Property<double>("Latitude")
                        .HasColumnType("double");

                    b.Property<string>("LicenseNumber")
                        .HasColumnType("varchar(100)");

                    b.Property<double>("Longitude")
                        .HasColumnType("double");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("OwnerName")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PrinterCOM")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ThumbImage")
                        .HasColumnType("TinyText");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Shop");
                });

            modelBuilder.Entity("ArumModels.Models.Stamp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Redeemed")
                        .HasColumnType("int");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.HasIndex("UserId");

                    b.ToTable("Stamp");
                });

            modelBuilder.Entity("ArumModels.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("IsAbleToUseCoupon")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsTempPassword")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("KakaoId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Token")
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("IngredientMenu", b =>
                {
                    b.Property<int>("IngredientListId")
                        .HasColumnType("int");

                    b.Property<int>("MenuListId")
                        .HasColumnType("int");

                    b.HasKey("IngredientListId", "MenuListId");

                    b.HasIndex("MenuListId");

                    b.ToTable("IngredientMenu");
                });

            modelBuilder.Entity("MenuOption", b =>
                {
                    b.Property<int>("MenuListId")
                        .HasColumnType("int");

                    b.Property<int>("OptionListId")
                        .HasColumnType("int");

                    b.HasKey("MenuListId", "OptionListId");

                    b.HasIndex("OptionListId");

                    b.ToTable("MenuOption");
                });

            modelBuilder.Entity("ArumModels.Models.CarouselImage", b =>
                {
                    b.HasOne("ArumModels.Models.Shop", null)
                        .WithMany("CarouselImages")
                        .HasForeignKey("ShopId");
                });

            modelBuilder.Entity("ArumModels.Models.Category", b =>
                {
                    b.HasOne("ArumModels.Models.Shop", null)
                        .WithMany("Categories")
                        .HasForeignKey("ShopId");
                });

            modelBuilder.Entity("ArumModels.Models.Coupon", b =>
                {
                    b.HasOne("ArumModels.Models.Shop", "Shop")
                        .WithMany()
                        .HasForeignKey("ShopId");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("ArumModels.Models.Menu", b =>
                {
                    b.HasOne("ArumModels.Models.Category", null)
                        .WithMany("MenuList")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("ArumModels.Models.Order", b =>
                {
                    b.HasOne("ArumModels.Models.Shop", null)
                        .WithMany("Orders")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArumModels.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ArumModels.Models.OrderedMenu", b =>
                {
                    b.HasOne("ArumModels.Models.Order", null)
                        .WithMany("MenuList")
                        .HasForeignKey("OrderId");
                });

            modelBuilder.Entity("ArumModels.Models.OrderedOption", b =>
                {
                    b.HasOne("ArumModels.Models.OrderedMenu", null)
                        .WithMany("OptionList")
                        .HasForeignKey("OrderedMenuId");
                });

            modelBuilder.Entity("ArumModels.Models.Stamp", b =>
                {
                    b.HasOne("ArumModels.Models.Shop", "Shop")
                        .WithMany()
                        .HasForeignKey("ShopId");

                    b.HasOne("ArumModels.Models.User", "User")
                        .WithMany("StampsList")
                        .HasForeignKey("UserId");

                    b.Navigation("Shop");

                    b.Navigation("User");
                });

            modelBuilder.Entity("IngredientMenu", b =>
                {
                    b.HasOne("ArumModels.Models.Ingredient", null)
                        .WithMany()
                        .HasForeignKey("IngredientListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArumModels.Models.Menu", null)
                        .WithMany()
                        .HasForeignKey("MenuListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MenuOption", b =>
                {
                    b.HasOne("ArumModels.Models.Menu", null)
                        .WithMany()
                        .HasForeignKey("MenuListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArumModels.Models.Option", null)
                        .WithMany()
                        .HasForeignKey("OptionListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArumModels.Models.Category", b =>
                {
                    b.Navigation("MenuList");
                });

            modelBuilder.Entity("ArumModels.Models.Order", b =>
                {
                    b.Navigation("MenuList");
                });

            modelBuilder.Entity("ArumModels.Models.OrderedMenu", b =>
                {
                    b.Navigation("OptionList");
                });

            modelBuilder.Entity("ArumModels.Models.Shop", b =>
                {
                    b.Navigation("CarouselImages");

                    b.Navigation("Categories");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("ArumModels.Models.User", b =>
                {
                    b.Navigation("StampsList");
                });
#pragma warning restore 612, 618
        }
    }
}
