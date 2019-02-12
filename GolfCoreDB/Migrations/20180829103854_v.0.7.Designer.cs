﻿// <auto-generated />
using GolfCoreDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GolfCoreDB.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20180829103854_v.0.7")]
    partial class v07
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastTask");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

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

                    b.Property<bool>("GetUpdates");

                    b.Property<bool>("MonitorUpdates");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameParticipant");
                });

            modelBuilder.Entity("GolfCoreDB.Data.KnownLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<decimal>("Lat");

                    b.Property<decimal>("Lon");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GolfCoreDB.Data.Setting", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ChatId");

                    b.Property<string>("GameId");

                    b.Property<string>("Value");

                    b.HasKey("Name");

                    b.ToTable("Settings");
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