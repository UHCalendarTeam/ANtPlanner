using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;
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
            var connection = @"Server=(localdb)\mssqllocaldb;Database=UHCalendarDB;Trusted_Connection=True;";
            //optionBuilder.UseSqlServer(connection);
            optionBuilder.UseInMemoryDatabase();
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarCollection>()
                .HasOne(u => u.User)
                .WithMany(cr => cr.CalendarCollections)
                .HasForeignKey(k => k.UserId);

            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.Collection)
                .WithMany(u => u.Calendarresources)
                .HasForeignKey(k => k.CollectionId);

            modelBuilder.Entity<CollectionProperty>()
                .HasOne(c => c.Collection)
                .WithMany(p => p.Properties)
                .HasForeignKey(k => k.CollectionId);

            modelBuilder.Entity<ResourceProperty>()
                .HasOne(r => r.Resource)
                .WithMany(p => p.Properties)
                .HasForeignKey(k => k.ResourceId);
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
