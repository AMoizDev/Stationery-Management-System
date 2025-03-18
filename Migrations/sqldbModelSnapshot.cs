﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stationery_Management_System.db_context;

#nullable disable

namespace Stationery_Management_System.Migrations
{
    [DbContext(typeof(sqldb))]
    partial class sqldbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Stationery_Management_System.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Stationery_Management_System.Models.Request", b =>
                {
                    b.Property<int>("requestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("requestId"));

                    b.Property<int>("amount")
                        .HasColumnType("int");

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.Property<int?>("stationaryId")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("superior_id")
                        .HasColumnType("int");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("requestId");

                    b.HasIndex("stationaryId");

                    b.HasIndex("superior_id");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("Stationery_Management_System.Models.Stationery", b =>
                {
                    b.Property<int>("Stationery_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Stationery_Id"));

                    b.Property<int?>("Assign_to")
                        .HasColumnType("int");

                    b.Property<string>("Stationery_Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stationery_Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stationery_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Stationery_Price")
                        .HasColumnType("int");

                    b.Property<int>("Stationery_Quantity")
                        .HasColumnType("int");

                    b.HasKey("Stationery_Id");

                    b.HasIndex("Assign_to");

                    b.ToTable("Stationeries");
                });

            modelBuilder.Entity("Stationery_Management_System.Models.UserRoles", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserRoleId"));

                    b.Property<string>("UserRoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserRoleId");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            UserRoleId = 1,
                            UserRoleName = "Admin"
                        });
                });

            modelBuilder.Entity("Stationery_Management_System.Models.users", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("userId"));

                    b.Property<int?>("Add_By")
                        .HasColumnType("int");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserLimits")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.HasKey("userId");

                    b.HasIndex("Add_By");

                    b.HasIndex("UserRole");

                    b.ToTable("users");

                    b.HasData(
                        new
                        {
                            userId = 1,
                            Add_By = 1,
                            UserEmail = "Admin@gmail.com",
                            UserLimits = 100000,
                            UserName = "Admin",
                            UserPassword = "123456789",
                            UserPhone = "03123456789",
                            UserRole = 1
                        });
                });

            modelBuilder.Entity("Stationery_Management_System.Models.Request", b =>
                {
                    b.HasOne("Stationery_Management_System.Models.Stationery", "Stationery")
                        .WithMany()
                        .HasForeignKey("stationaryId");

                    b.HasOne("Stationery_Management_System.Models.users", "Users")
                        .WithMany()
                        .HasForeignKey("superior_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stationery");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Stationery_Management_System.Models.Stationery", b =>
                {
                    b.HasOne("Stationery_Management_System.Models.UserRoles", "Roles")
                        .WithMany()
                        .HasForeignKey("Assign_to");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Stationery_Management_System.Models.users", b =>
                {
                    b.HasOne("Stationery_Management_System.Models.users", "AddedBy")
                        .WithMany()
                        .HasForeignKey("Add_By");

                    b.HasOne("Stationery_Management_System.Models.UserRoles", "Roles")
                        .WithMany()
                        .HasForeignKey("UserRole")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AddedBy");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
