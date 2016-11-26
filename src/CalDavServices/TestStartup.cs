using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using DataLayer.Models.Entities;
using DataLayer.Repositories.Implementations;
using DataLayer.Models.ACL;
using DataLayer.Repositories;

namespace CalDavServices
{
    /// <summary>
    /// This class is used for test porpoise only.
    /// It is in charge of configure the system for a test environment.
    /// </summary>
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        public override void ConfigureDatabase(IServiceCollection services)
        {
            //var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            //var connectionString = connectionStringBuilder.ToString();
            //var connection = new SqliteConnection(connectionString);
            //services
            //  .AddEntityFrameworkSqlite()
            //  .AddDbContext<CalDavContext>(
            //    options => options.UseSqlite(connection)
            //  );
            services.AddDbContext<CalDavContext>(
                optionsBuilder => optionsBuilder.UseInMemoryDatabase());
        }

        private async Task MockDatabase(CalDavContext _context)
        {
            #region FIlling Database

            //FileManagement fs = new FileManagement();            

            // This is the magic line
            //  optionsBuilder.UseInMemoryDatabase();  
            #region User
            var user = new User("admin@admin.uh.cu", "admin@admin.uh.cu", "AQAAAAEAACcQAAAAEPHS/DMuYco3Ny3d2Y3iczPAT6nZ2C7rh/JDs5L5MhrmUGmgdLLNWwLjo/Ecw52Wqw==");
            #endregion

            #region Principal

            var displayName = PropertyCreation.CreateProperty("displayname", "D", user.DisplayName);

            var principal = new Principal("admin@admin.uh.cu", SystemProperties.PrincipalType.User);
            #endregion

            #region resource
            var resources = new List<CalendarResource>
                        {
                            new CalendarResource("/collections/groups/public/C212/test.ics", "test.ics")

                        };
            #endregion            

            #region CalendarHome
            var calHome = CalendarHomeRepository.CreateCalendarHome(principal);


            //var homeCollection = new CalendarHome("/collections/groups/public/", "PubicCollections");
            #endregion

            user.Principal = principal;

            var calHomeSet = PropertyCreation.CreateCalHomeSetWithHref(calHome.Url);

            principal.Properties.Add(calHomeSet);

            principal.CalendarHome = calHome;

            //principal.CalendarCollections = collection;



            //user.Resources = resources;
            _context.Users.Add(user);
            _context.Principals.Add(principal);
            _context.CalendarHomeCollections.Add(calHome);
            _context.CalendarCollections.AddRange(calHome.CalendarCollections);
            await _context.SaveChangesAsync();

            var collectionRepo = new CollectionRepository(_context);

            var collectionC212 = collectionRepo.Get("/collections/groups/public/C212/");

            collectionC212.CalendarResources.Add(resources[0]);

            await _context.SaveChangesAsync();


            #endregion
        }

        public override void ConfigureInDev(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var context = app.ApplicationServices.GetService<CalDavContext>();
                MockDatabase(context).Wait();
            }
        }        

    }
}
