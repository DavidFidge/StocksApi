﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StocksApi.Data;

namespace StocksApi.Data.Migrations
{
    [DbContext(typeof(StocksContext))]
    [Migration("20200726044939_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("StocksApi.Model.EndOfDay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Close")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("High")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Low")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Open")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("StockId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Volume")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("StockId");

                    b.ToTable("EndOfDay");
                });

            modelBuilder.Entity("StocksApi.Model.Holding", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Brokerage")
                        .HasColumnType("TEXT");

                    b.Property<long>("NumberOfShares")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("PortfolioId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("StockId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioId");

                    b.HasIndex("StockId");

                    b.ToTable("Holding");
                });

            modelBuilder.Entity("StocksApi.Model.Portfolio", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PortfolioManagerId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioManagerId");

                    b.ToTable("Portfolio");
                });

            modelBuilder.Entity("StocksApi.Model.PortfolioManager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PortfolioManager");
                });

            modelBuilder.Entity("StocksApi.Model.Stock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("CompanyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("IndustryGroup")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Stock");
                });

            modelBuilder.Entity("StocksApi.Model.EndOfDay", b =>
                {
                    b.HasOne("StocksApi.Model.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId");
                });

            modelBuilder.Entity("StocksApi.Model.Holding", b =>
                {
                    b.HasOne("StocksApi.Model.Portfolio", null)
                        .WithMany("Holdings")
                        .HasForeignKey("PortfolioId");

                    b.HasOne("StocksApi.Model.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId");
                });

            modelBuilder.Entity("StocksApi.Model.Portfolio", b =>
                {
                    b.HasOne("StocksApi.Model.PortfolioManager", null)
                        .WithMany("Portfolios")
                        .HasForeignKey("PortfolioManagerId");
                });
#pragma warning restore 612, 618
        }
    }
}
