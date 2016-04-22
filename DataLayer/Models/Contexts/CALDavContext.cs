using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace DataLayer
{
    public class CalDavContext : DbContext
    {
        public readonly IConfigurationRoot Configuration;

        /// <summary>
        /// Contains the url for the user's collections
        /// Add the email of the user
        /// </summary>
        public readonly string _userCollectionUrl = "collections/users/";

        /// <summary>
        /// Contains the url for the groups collection.
        /// Add the name of the group
        /// </summary>
        public readonly string _groupCollectionUrl = "collections/group/";

        /// <summary>
        /// Contains the default name for the user collections
        /// </summary>
        public readonly string _defualtInitialCollectionName = "DefualCalendar";



        public CalDavContext(DbContextOptions options)
            : base(options)
        {
           
        }
        /// <summary>
        /// inject the configuration of the app to be accessible in the DataLayer
        /// </summary>
        /// <param name="config"></param>
        public CalDavContext()
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Worker> Workers { get; set; }
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