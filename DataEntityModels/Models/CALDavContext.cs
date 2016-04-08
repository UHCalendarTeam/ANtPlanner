using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace DataEntityModels
{
    public class CalDavContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CalendarCollection> CalendarCollections { get; set; }

        public DbSet<CalendarResource> CalendarResources { get; set; }

      /*  protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Caldav;Trusted_Connection=True;");

        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarCollection>()
                .HasOne(u => u.User)
                .WithMany(cr => cr.CalendarCollections);
            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.User)
                .WithMany(u => u.Resources);
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
