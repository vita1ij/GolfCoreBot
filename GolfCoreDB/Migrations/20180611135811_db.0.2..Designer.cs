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
    [Migration("20180611135811_db.0.2.")]
    partial class db02
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

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameParticipant");
                });

            modelBuilder.Entity("GolfCoreDB.Data.Setting", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChatId");

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
