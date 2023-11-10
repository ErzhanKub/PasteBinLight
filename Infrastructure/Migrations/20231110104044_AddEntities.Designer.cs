﻿// <auto-generated />
using System;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231110104044_AddEntities")]
    partial class AddEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Record", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DeadLine")
                        .HasColumnType("datetime2");

                    b.Property<long>("DisLikes")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<long>("Likes")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Records");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a3af1c84-8977-474e-b475-524f96499018"),
                            DateCreated = new DateTime(2023, 11, 10, 16, 40, 44, 849, DateTimeKind.Local).AddTicks(9712),
                            DeadLine = new DateTime(2023, 12, 10, 16, 40, 44, 849, DateTimeKind.Local).AddTicks(9730),
                            DisLikes = 13L,
                            IsPrivate = false,
                            Likes = 183L,
                            Title = "My day",
                            Url = "https://www.youtube.com/",
                            UserId = new Guid("7a3fad07-ec50-42ec-bd8c-c083b9f342b8")
                        });
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConfirmationToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7a3fad07-ec50-42ec-bd8c-c083b9f342b8"),
                            ConfirmationToken = "ConfirmToken",
                            Email = "string@mail.com",
                            Password = "b4923143305b8c19f5c5031c406be92b99ce221e00c795598356d9fc0fc117bc",
                            Role = 2,
                            Username = "SuperAdmin2077CP"
                        });
                });

            modelBuilder.Entity("Domain.Entities.Record", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Records")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("Records");
                });
#pragma warning restore 612, 618
        }
    }
}