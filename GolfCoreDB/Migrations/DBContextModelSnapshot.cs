﻿// <auto-generated />
using System;
using GolfCoreDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GolfCoreDB.Migrations
{
    [DbContext(typeof(DBContext))]
    partial class DBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GolfCoreDB.Data.Game", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EnCxId");

                    b.Property<string>("EnCxType");

                    b.Property<string>("Href");

                    b.Property<bool>("IsActive");

                    b.Property<byte[]>("LastStatistics");

                    b.Property<int?>("LastStatisticsHash");

                    b.Property<string>("LastTask");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<string>("Prefix");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GolfCoreDB.Data.GameParticipant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId");

                    b.Property<string>("GameId");

                    b.Property<bool>("GetStatistics");

                    b.Property<bool>("GetUpdates");

                    b.Property<bool>("MonitorStatistics");

                    b.Property<bool>("MonitorUpdates");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameParticipant");
                });

            modelBuilder.Entity("GolfCoreDB.Data.GameTask", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GameId");

                    b.Property<string>("TaskText");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("GolfCoreDB.Data.KnownLocation", b =>
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

            modelBuilder.Entity("GolfCoreDB.Data.GameParticipant", b =>
                {
                    b.HasOne("GolfCoreDB.Data.Game", "Game")
                        .WithMany("Participants")
                        .HasForeignKey("GameId");
                });
#pragma warning restore 612, 618
        }
    }
}
