﻿// <auto-generated />
using System;
using M3alam.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace M3alam.Infrastructure.Migrations
{
    [DbContext(typeof(M3alamDbContext))]
    [Migration("20250508100524_UpdateMigration")]
    partial class UpdateMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("M3alam.Domain.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("RowVersion")
                        .HasColumnType("varbinary(max)");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Rating", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("RowVersion")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<Guid>("SeekerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.HasIndex("RequestId");

                    b.HasIndex("SeekerId");

                    b.ToTable("Rating");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Request", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<DateTime>("PreferredTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("RowVersion")
                        .HasColumnType("varbinary(max)");

                    b.Property<Guid>("SeekerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ServiceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.HasIndex("SeekerId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("M3alam.Identity.Infrastructure.Identity.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUser");

                    b.HasDiscriminator().HasValue("ApplicationUser");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Provider", b =>
                {
                    b.HasBaseType("M3alam.Identity.Infrastructure.Identity.ApplicationUser");

                    b.Property<string>("Availability")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CompletedJobs")
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<int>("ReviewCount")
                        .HasColumnType("int");

                    b.Property<string>("Skills")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Provider");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Seeker", b =>
                {
                    b.HasBaseType("M3alam.Identity.Infrastructure.Identity.ApplicationUser");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("Seeker");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Message", b =>
                {
                    b.HasOne("M3alam.Domain.Entities.Request", "Request")
                        .WithMany("Messages")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("M3alam.Identity.Infrastructure.Identity.ApplicationUser", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Rating", b =>
                {
                    b.HasOne("M3alam.Domain.Entities.Provider", "Provider")
                        .WithMany("RatingsReceived")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("M3alam.Domain.Entities.Request", "Request")
                        .WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("M3alam.Domain.Entities.Seeker", "Seeker")
                        .WithMany("RatingsGiven")
                        .HasForeignKey("SeekerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");

                    b.Navigation("Request");

                    b.Navigation("Seeker");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Request", b =>
                {
                    b.HasOne("M3alam.Domain.Entities.Provider", "Provider")
                        .WithMany("RequestsHandled")
                        .HasForeignKey("ProviderId");

                    b.HasOne("M3alam.Domain.Entities.Seeker", "Seeker")
                        .WithMany("Requests")
                        .HasForeignKey("SeekerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");

                    b.Navigation("Seeker");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Request", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Provider", b =>
                {
                    b.Navigation("RatingsReceived");

                    b.Navigation("RequestsHandled");
                });

            modelBuilder.Entity("M3alam.Domain.Entities.Seeker", b =>
                {
                    b.Navigation("RatingsGiven");

                    b.Navigation("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
