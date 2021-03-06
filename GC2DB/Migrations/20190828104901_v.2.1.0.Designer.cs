﻿// <auto-generated />
using System;
using GC2DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GC2DB.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20190828104901_v.2.1.0")]
    partial class v210
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GC2DB.Data.DataFile", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("ChatId");

                    b.Property<byte[]>("Content");

                    b.Property<int?>("LocationId");

                    b.Property<long?>("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("TaskId");

                    b.ToTable("DataFiles");
                });

            modelBuilder.Entity("GC2DB.Data.Game", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CenterCoordinates");

                    b.Property<string>("EnCxId");

                    b.Property<string>("EnCxType");

                    b.Property<string>("Guid");

                    b.Property<string>("Href");

                    b.Property<string>("LastTask");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<string>("Prefix");

                    b.Property<long?>("Radius");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.Property<bool>("isActive");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GC2DB.Data.ListItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("Lists");
                });

            modelBuilder.Entity("GC2DB.Data.Location", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<double>("Lat");

                    b.Property<double>("Lon");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GC2DB.Data.Player", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId");

                    b.Property<long?>("GameId");

                    b.Property<int>("UpdateStatisticsInfo");

                    b.Property<int>("UpdateTaskInfo");

                    b.Property<bool>("isActive");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("GC2DB.Data.PlayersLocation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId");

                    b.Property<int?>("LocationId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("PlayersLocation");
                });

            modelBuilder.Entity("GC2DB.Data.Task", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("GameId");

                    b.Property<long?>("Number");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("GC2DB.Data.DataFile", b =>
                {
                    b.HasOne("GC2DB.Data.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("GC2DB.Data.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId");
                });

            modelBuilder.Entity("GC2DB.Data.Player", b =>
                {
                    b.HasOne("GC2DB.Data.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("GC2DB.Data.PlayersLocation", b =>
                {
                    b.HasOne("GC2DB.Data.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("GC2DB.Data.Task", b =>
                {
                    b.HasOne("GC2DB.Data.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");
                });
#pragma warning restore 612, 618
        }
    }
}
