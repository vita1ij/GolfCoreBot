using GC2DB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace GC2DB
{
    public class DBContext : DbContext, IDisposable
    {
        #pragma warning disable CS8618 // Non-nullable field is uninitialized.
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<GameTask> Tasks { get; set; }
        public DbSet<DataFile> DataFiles { get; set; }
        public DbSet<ListItem> Lists { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PlayersLocation> PlayersLocation { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<AcmeData> AcmeValues { get; set; } 
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        #region Methods
        public static DBContext Instance
        {
            get
            {
                return new DBContext();
            }
        }

        public static IConfiguration Config
        {
            get
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                return new ConfigurationBuilder()
                    .SetBasePath(projectPath)
                    .AddJsonFile("config.json")
                    .Build();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string connectionString = Config.GetValue(typeof(String), "CONNECTION_STRING").ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //foreach (var property in modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetProperties())
            //    .Where(p => p.ClrType == typeof(decimal)))
            //{
            //    property.Relational().ColumnType = "decimal(10, 6)";
            //}
            //todo[vg] relational

            base.OnModelCreating(modelBuilder);
        }
        #endregion 
    }
}
