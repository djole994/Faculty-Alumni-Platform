using System.Collections.Generic;
using AlumniApi.Models.AlProfile;
using AlumniApi.Models.Auth;
using AlumniApi.Models.Caching;
using Microsoft.EntityFrameworkCore;
using AlumniApi.Models;

namespace AlumniApi.Models
{
    public class AlumniContext : DbContext
    {
        public AlumniContext(DbContextOptions<AlumniContext> options) : base(options) { }

        public DbSet<AlumniProfile> AlumniProfiles => Set<AlumniProfile>();
        public DbSet<GeoCache> GeoCaches => Set<GeoCache>();
        public DbSet<Country> Countries => Set<Country>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. KONFIGURACIJA ZA ALUMNIPROFILE ---
            modelBuilder.Entity<AlumniProfile>(entity =>
            {
                entity.ToTable("AlumniProfiles");

                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Country)
                      .WithMany()
                      .HasForeignKey(p => p.CountryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.GeoCache)
                      .WithMany()
                      .HasForeignKey(p => p.GeoCacheId)
                      .OnDelete(DeleteBehavior.SetNull); 

                entity.HasIndex(p => p.ContactEmail).IsUnique();

                entity.Property(p => p.DateOfBirth).HasColumnType("date");
                entity.Property(p => p.GraduationDate).HasColumnType("date");

                entity.HasIndex(p => p.CountryId);
                entity.HasIndex(p => p.City);
                entity.HasIndex(p => p.IsLocationVerified);
                entity.HasIndex(p => p.IsApproved);

                entity.Property(p => p.ContactEmail).HasMaxLength(256);
                entity.Property(p => p.FullName).HasMaxLength(200);
                entity.Property(p => p.City).HasMaxLength(120);
            });

            // --- 2. KONFIGURACIJA ZA GeoCache ---
            modelBuilder.Entity<GeoCache>(entity =>
            {
                    entity.HasIndex(x => x.SearchKey).IsUnique();
            });

            // --- 3. KONFIGURACIJA ZA COUNTRY ---
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
                entity.Property(c => c.Name).HasMaxLength(100).IsRequired();

                entity.HasIndex(c => c.IsoCode).IsUnique();
                entity.Property(c => c.IsoCode).IsRequired().HasMaxLength(5);
            });
        }
    }
}
