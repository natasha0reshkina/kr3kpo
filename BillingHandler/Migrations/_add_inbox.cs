
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using BillingService.Persistence;
#nullable disable
namespace BillingService.Migrations
{
    [DbContext(typeof(PaymentsDbContext))]
    [Migration("20250610134743_AddInboxMessages")]
    partial class AddInboxMessages
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
            modelBuilder.Entity("BillingService.Persistence.Entities.Account", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");
                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.HasKey("UserId");
                    b.ToTable("Accounts");
                });
            modelBuilder.Entity("BillingService.Persistence.Entities.InboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");
                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid");
                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("text");
                    b.Property<DateTime?>("ProcessedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<DateTime>("ReceivedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");
                    b.HasKey("Id");
                    b.ToTable("InboxMessages");
                });
            modelBuilder.Entity("BillingService.Persistence.Entities.OutboxMessage", b =>
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