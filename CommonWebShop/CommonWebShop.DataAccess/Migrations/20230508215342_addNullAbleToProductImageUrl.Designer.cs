﻿// <auto-generated />
using CommonWebShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CommonWebShop.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230508215342_addNullAbleToProductImageUrl")]
    partial class addNullAbleToProductImageUrl
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CommonWebShop.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Shonen"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "Shojo"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Josei"
                        });
                });

            modelBuilder.Entity("CommonWebShop.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price10")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "Takao",
                            CategoryId = 1,
                            Description = "Manga",
                            ImageUrl = "",
                            Price = 20.0,
                            Price10 = 15.0,
                            Title = "Teneno"
                        },
                        new
                        {
                            Id = 2,
                            Author = "Sakao",
                            CategoryId = 2,
                            Description = "Manga",
                            ImageUrl = "",
                            Price = 20.0,
                            Price10 = 15.0,
                            Title = "Sakeneo"
                        },
                        new
                        {
                            Id = 3,
                            Author = "Kokao",
                            CategoryId = 3,
                            Description = "Manga",
                            ImageUrl = "",
                            Price = 20.0,
                            Price10 = 15.0,
                            Title = "Ono"
                        });
                });

            modelBuilder.Entity("CommonWebShop.Models.Product", b =>
                {
                    b.HasOne("CommonWebShop.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}