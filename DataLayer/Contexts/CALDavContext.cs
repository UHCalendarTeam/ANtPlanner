using System.Diagnostics;
using System.IO;
using System.Linq;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Entities.OtherEnt.RelationsEnt;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;


namespace DataLayer
{
    public class CalDavContext : DbContext
    {
        public readonly IConfigurationRoot Configuration;

        public CalDavContext()
        {
            // Database.EnsureCreated();
        }

        public CalDavContext(DbContextOptions options)
            : base(options)
        {
            // Database.EnsureCreated();
        }

        /// <summary>
        ///     inject the configuration of the app to be accessible in the DataLayer
        /// </summary>
        public DbSet<User> Users { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Worker> Workers { get; set; }

        public DbSet<CalendarHome> CalendarHomeCollections { get; set; }

        public DbSet<CalendarCollection> CalendarCollections { get; set; }

        public DbSet<CalendarResource> CalendarResources { get; set; }

        public DbSet<Principal> Principals { get; set; }

        public DbSet<Property> Properties { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<ResourceType> ResourceTypes { get; set; }

        public DbSet<FileImage> ImageFile { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            if (!optionBuilder.IsConfigured)
            {
                #region SQLServer            
                //optionBuilder.UseSqlServer(SystemProperties.SQLServerConnectionString());
                //optionBuilder.UseInMemoryDatabase();
                #endregion

                #region SQLite
                //var path = PlatformServices.Default.Application.ApplicationBasePath;
                //var connection = "Filename=" + Path.Combine(path, "UHCalendar.db");
                //optionBuilder.UseSqlite(connection);
                #endregion

                #region Npgsql
                //optionBuilder.UseNpgsql(SystemProperties.NpgsqlConnectionString());
                optionBuilder.UseNpgsql(SystemProperties.NpgsqlConnectionString());//testing in my pc yasmany
                #endregion
            }


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region First relations

            modelBuilder.Entity<Principal>()
                .HasOne(p => p.User)
                .WithOne(u => u.Principal)
                .HasForeignKey<User>(u => u.PrincipalId);

            modelBuilder.Entity<Principal>().HasAlternateKey(p => p.PrincipalUrl);

            modelBuilder.Entity<Principal>()
            .HasOne(p => p.CalendarHome)
            .WithOne(i => i.Principal)
            .HasForeignKey<CalendarHome>(c => c.PrincipalId);

            //modelBuilder.Entity<CalendarCollection>()
            //    .HasOne(u => u.Principal)
            //    .WithMany(cl => cl.CalendarCollections)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<CalendarCollection>().HasAlternateKey(c => c.Url);

            modelBuilder.Entity<CalendarResource>()
                .HasOne(cl => cl.CalendarCollection)
                .WithMany(u => u.CalendarResources)
                .HasForeignKey(k => k.CalendarCollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CalendarCollection>()
                .HasOne(cl => cl.CalendarHome)
                .WithMany(u => u.CalendarCollections)
                .HasForeignKey(k => k.CalendarHomeId)//change by yasmay
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

            modelBuilder.Entity<Property>()
                .HasOne(p => p.CalendarHome)
                .WithMany(ch => ch.Properties)
                .HasForeignKey(k => k.CalendarHomeId);

            modelBuilder.Entity<ResourceType>()
                .HasMany(p => p.Resources)
                .WithOne(r => r.ResourceType)
                .HasForeignKey(rt => rt.ResourceId);

            #endregion

            #region Many to Many relations

            //imagen and calendarResources
            modelBuilder.Entity<RCalendarResourcesImagenFiles>()
                .HasKey(t => new { t.ImageFilesId, t.CalendarResourceId });

            modelBuilder.Entity<RCalendarResourcesImagenFiles>()
                .HasOne(pt => pt.FileImage)
                .WithMany(p => p.RCalendarResourcesImagenFiles)
                .HasForeignKey(pt => pt.ImageFilesId);

            modelBuilder.Entity<RCalendarResourcesImagenFiles>()
                .HasOne(pt => pt.CalendarResource)
                .WithMany(t => t.RImageFilesResources)
                .HasForeignKey(pt => pt.CalendarResourceId);

            // Imagens and resources
            modelBuilder.Entity<RImagenFilesResources>()
                .HasKey(t => new { t.ImageFilesId, t.ResourceId });

            modelBuilder.Entity<RImagenFilesResources>()
                .HasOne(pt => pt.FileImage)
                .WithMany(p => p.RImagensFilesResources)
                .HasForeignKey(pt => pt.ImageFilesId);

            modelBuilder.Entity<RImagenFilesResources>()
                .HasOne(pt => pt.Resource)
                .WithMany(t => t.RImagenFilesResource)
                .HasForeignKey(pt => pt.ResourceId);

            //Imagens and Persons

            modelBuilder.Entity<RImagenFilesPersons>()
                .HasKey(t => new { ImageFilesId = t.ImageFileId, t.PersonId });

            modelBuilder.Entity<RImagenFilesPersons>()
                .HasOne(pt => pt.FileImage)
                .WithMany(p => p.RImagensFilesPersons)
                .HasForeignKey(pt => pt.ImageFileId);

            modelBuilder.Entity<RImagenFilesPersons>()
                .HasOne(pt => pt.Person)
                .WithMany(t => t.RImagenFilesPersons)
                .HasForeignKey(pt => pt.PersonId);


            // Imagen and Locations

            modelBuilder.Entity<RImagenFilesLocations>()
                .HasKey(t => new { t.ImageFilesId, t.LocationId });

            modelBuilder.Entity<RImagenFilesLocations>()
                .HasOne(pt => pt.FileImage)
                .WithMany(p => p.RImagenFilesLocations)
                .HasForeignKey(pt => pt.ImageFilesId);

            modelBuilder.Entity<RImagenFilesLocations>()
                .HasOne(pt => pt.Location)
                .WithMany(t => t.RImagenFilesLocations)
                .HasForeignKey(pt => pt.LocationId);

            //calendarResource and Location

            modelBuilder.Entity<RCalendarResourceLocation>()
                .HasKey(t => new { t.CalendarResoureId, t.LocationId });

            modelBuilder.Entity<RCalendarResourceLocation>()
                .HasOne(pt => pt.CalendarResource)
                .WithMany(p => p.RCalendarResourceLocations)
                .HasForeignKey(pt => pt.CalendarResoureId);

            modelBuilder.Entity<RCalendarResourceLocation>()
                .HasOne(pt => pt.Location)
                .WithMany(t => t.RCalendarResourceLocations)
                .HasForeignKey(pt => pt.LocationId);

            //Person location
            modelBuilder.Entity<RPersonLocation>()
                .HasKey(t => new { t.PersonId, t.LocationId });

            modelBuilder.Entity<RPersonLocation>()
                .HasOne(pt => pt.Person)
                .WithMany(p => p.RPersonLocations)
                .HasForeignKey(pt => pt.PersonId);

            modelBuilder.Entity<RPersonLocation>()
                .HasOne(pt => pt.Location)
                .WithMany(t => t.RPersonLocation)
                .HasForeignKey(pt => pt.LocationId);

            // Person and Resource

            modelBuilder.Entity<RPersonResource>()
                .HasKey(t => new { t.PersonId, t.ResourceId });

            modelBuilder.Entity<RPersonResource>()
                .HasOne(pt => pt.Person)
                .WithMany(p => p.RPersonResource)
                .HasForeignKey(pt => pt.PersonId);

            modelBuilder.Entity<RPersonResource>()
                .HasOne(pt => pt.Resource)
                .WithMany(t => t.RPersonResource)
                .HasForeignKey(pt => pt.ResourceId);

            // Calendar Resource and Person

            modelBuilder.Entity<RCalendarResourcePerson>()
           .HasKey(t => new { t.PersonId, t.CalendarResourceId });

            modelBuilder.Entity<RCalendarResourcePerson>()
                .HasOne(pt => pt.Person)
                .WithMany(p => p.RCalendarResourcePerson)
                .HasForeignKey(pt => pt.PersonId);

            modelBuilder.Entity<RCalendarResourcePerson>()
                .HasOne(pt => pt.CalendarResource)
                .WithMany(t => t.RCalendarResourcePersons)
                .HasForeignKey(pt => pt.CalendarResourceId);

            // Location and Resource
            modelBuilder.Entity<RLocationResource>()
                .HasKey(t => new { t.LocationId, t.ResourceId });

            modelBuilder.Entity<RLocationResource>()
                .HasOne(pt => pt.Location)
                .WithMany(p => p.RLocationResources)
                .HasForeignKey(pt => pt.LocationId);

            modelBuilder.Entity<RLocationResource>()
                .HasOne(pt => pt.Resource)
                .WithMany(t => t.RLocationResources)
                .HasForeignKey(pt => pt.ResourceId);

            //Resource and CalendarResource
            modelBuilder.Entity<RResourceCalendarResource>()
                .HasKey(t => new { t.CalendarResourceId, t.ResourceId });

            modelBuilder.Entity<RResourceCalendarResource>()
                .HasOne(pt => pt.Resource)
                .WithMany(p => p.ResourceCalendarResources)
                .HasForeignKey(pt => pt.ResourceId);

            modelBuilder.Entity<RResourceCalendarResource>()
                .HasOne(pt => pt.CalendarResource)
                .WithMany(t => t.RResourceCalendarResources)
                .HasForeignKey(pt => pt.CalendarResourceId);



            //CalendarResource and CalendarResource
            //todo: ver como hacer la realicion de muchos a muchos en una misma identidad.
            //modelBuilder.Entity<RCalendarResourcesCalendarResource>()
            //  .HasKey(t => new { t.CalendarResource1Id, t.CalendarResource2Id });

            //modelBuilder.Entity<RCalendarResourcesCalendarResource>()
            //    .HasOne(pt => pt.CalendarResource1)
            //    .WithMany(p => p.RCalendarResourcesCalendarResources)
            //    .HasForeignKey(pt => pt.CalendarResource1Id);
            #endregion

        }
    }
}