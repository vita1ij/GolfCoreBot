using GolfCoreDB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace GolfCoreDB
{
    public class DBContext : DbContext
    {
        public static DBContext Instance {
            get
            {
                return new DBContext();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                string connectionString = Config.GetConnectionString("GolfDB");
                optionsBuilder.UseSqlServer(connectionString);
            }
            catch (Exception ex)
            {

            }
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<KnownLocation> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal)))
            {
                property.Relational().ColumnType = "decimal(10, 6)";
            }

            base.OnModelCreating(modelBuilder);
        }

        public static IConfiguration Config
        {
            get
            {
                string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
                return new ConfigurationBuilder()
                    .SetBasePath(projectPath)
                    .AddJsonFile("config.json")
                    .Build();
            }
        }
    }
}
