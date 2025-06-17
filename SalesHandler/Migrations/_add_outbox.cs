﻿
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SalesService.Persistence;
#nullable disable
namespace SalesService.Migrations
{
    [DbContext(typeof(OrdersDbContext))]
    [Migration("20250610133856_AddOutboxMessages")]
    partial class AddOutboxMessages
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
            modelBuilder.Entity("SalesService.Persistence.Entities.Purchase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");
                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");
                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");
                    b.HasKey("Id");
                    b.ToTable("Purchases");
                });
            modelBuilder.Entity("SalesService.Persistence.Entities.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");
                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("text");
                    b.Property<DateTime?>("ProcessedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");
                    b.HasKey("Id");
                    b.ToTable("OutboxMessages");
                });
#pragma warning restore 612, 618
        }
    }
}