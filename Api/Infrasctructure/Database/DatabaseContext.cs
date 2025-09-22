using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Database
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configurationAppSettings) : DbContext(options)
    {
        public DbSet<Administrator> Administrators => Set<Administrator>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator
                {
                    Id = 1,
                    Email = "administrator@minimalapi.com",
                    Password = "12345678",
                    Profile = Profile.ADMINISTRATOR
                },
                new Administrator
                {
                    Id = 2,
                    Email = "editor@minimalapi.com",
                    Password = "12345678",
                    Profile = Profile.EDITOR
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (options.IsConfigured) return;

            string connectionString = configurationAppSettings.GetConnectionString("Mysql") ?? null!;

            if (!string.IsNullOrEmpty(connectionString)) return;

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}