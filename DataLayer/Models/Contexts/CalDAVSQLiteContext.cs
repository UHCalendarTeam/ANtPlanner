using System.IO;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace DataLayer
{
    public class CalDAVSQLiteContext : DbContext
    {
        public readonly IConfigurationRoot Configuration;


        public CalDAVSQLiteContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        ///     inject the configuration of the app to be accessible in the DataLayer
        /// </summary>
        public CalDAVSQLiteContext()
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<CalendarCollection> CalendarCollections { get; set; }

        public DbSet<CalendarResource> CalendarResources { get; set; }

        public DbSet<Principal> Principals { get; set; }

        public DbSet<Property> Properties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            var path = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "UHCalendarDb");
            var connection = "Filename=" + Path.Combine(path, "UHCalendar.db");
            optionBuilder.UseSqlite(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Principal>()
                .HasOne(p => p.User)
                .WithOne(u => u.Principal)
                .HasForeignKey<User>(u => u.PrincipalId);

            modelBuilder.Entity<Principal>().HasAlternateKey(p => p.PrincipalURL);

            modelBuilder.Entity<CalendarCollection>()
                .HasOne(u => u.Principal)
                .WithMany(cl => cl.CalendarCollections)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CalendarCollection>().HasAlternateKey(c => c.Url);

            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.CalendarCollection)
                .WithMany(u => u.CalendarResources)
                .HasForeignKey(k => k.CalendarCollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CalendarResource>().HasAlternateKey(c => c.Href);

            modelBuilder.Entity<Property>()
                .HasOne(c => c.CalendarCollection)
                .WithMany(p => p.Properties)
                .HasForeignKey(k => k.CalendarCollectionId);
            
            modelBuilder.Entity<Property>()
                .HasOne(r => r.CalendarResource)
                .WithMany(p => p.Properties)
                .HasForeignKey(k => k.CalendarResourceId);


            modelBuilder.Entity<Property>()
                .HasOne(p => p.Principal)
                .WithMany(pr => pr.Properties)
                .HasForeignKey(k => k.PricipalId);
        }
    }
}
