using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace DataLayer
{
    public class CalDavContext : DbContext
    {
        public CalDavContext(DbContextOptions options)
            : base(options)
        {
        }

        public CalDavContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CalendarCollection> CalendarCollections { get; set; }

        public DbSet<CalendarResource> CalendarResources { get; set; }

        public DbSet<Principal> Principals { get; set; }
        public DbSet<Property> CollectionProperties { get; set; }

        public DbSet<Property> ResourceProperties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            var connection = @"Server=(localdb)\mssqllocaldb;Database=UHCalendarDB;Trusted_Connection=True;";
            optionBuilder.UseSqlServer(connection);
            //optionBuilder.UseInMemoryDatabase();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarCollection>()
                .HasOne(u => u.User)
                .WithMany(cr => cr.CalendarCollections)
                .HasForeignKey(k => k.UserId);

            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.CalendarCollection)
                .WithMany(u => u.CalendarResources)
                .HasForeignKey(k => k.CalendarCollectionId);

            modelBuilder.Entity<Property>()
                .HasOne(c => c.Collection)
                .WithMany(p => p.Properties)
                .HasForeignKey(k => k.CollectionId);

            modelBuilder.Entity<Property>()
                .HasOne(r => r.Resource)
                .WithMany(p => p.Properties)
                .HasForeignKey(k => k.ResourceId);
        }
    }
}