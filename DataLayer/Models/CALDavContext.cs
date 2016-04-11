using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace DataLayer
{
    public class CalDavContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CalendarCollection> CalendarCollections { get; set; }

        public DbSet<CalendarResource> CalendarResources { get; set; }

        public DbSet<CollectionProperty> CollectionProperties { get; set; }

        public DbSet<ResourceProperty> ResourceProperties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            var connection = @"Server=(localdb)\mssqllocaldb;Database=UHCalendarDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            optionBuilder.UseSqlServer(connection).MigrationsAssembly("DataLayer");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarCollection>()
                .HasOne(u => u.User)
                .WithMany(cr => cr.CalendarCollections);

            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.Collection)
                .WithMany(u => u.Calendarresources);

            modelBuilder.Entity<CollectionProperty>()
                .HasOne(c => c.Collection)
                .WithMany(p => p.Properties);

            modelBuilder.Entity<ResourceProperty>()
                .HasOne(r => r.Resource)
                .WithMany(p => p.Properties);
        }

        public CalDavContext(DbContextOptions options)
            : base(options)
        {
        }

        public CalDavContext()
        {
        }
    }


}
