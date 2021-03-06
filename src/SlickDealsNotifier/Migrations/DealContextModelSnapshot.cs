﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SlickdealsNotifier.Models;

namespace SlickdealsNotifier.Migrations
{
    [DbContext(typeof(DealContext))]
    partial class DealContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("SlickdealsNotifier.Models.Deal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsFire")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Price")
                        .HasColumnType("TEXT");

                    b.Property<string>("Store")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<int>("Votes")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Deals");
                });
#pragma warning restore 612, 618
        }
    }
}
