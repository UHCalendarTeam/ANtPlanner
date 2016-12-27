using System.IO;
using System;
using ACL.Core;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDavServices.Extensions;
using CalDAV.Core;
using CalDAV.Core.ConditionsCheck.Preconditions;
using CalDAV.Core.ConditionsCheck.Preconditions.Report;
using DataLayer;
using DataLayer.Contexts;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;
using Serilog.Sinks.RollingFile;

namespace CalDavServices
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Warning()
        .WriteTo.RollingFile(Path.Combine("appLogs", "log-{Date}.txt"))
        .CreateLogger();
            SystemProperties.AbsolutePath = env.ContentRootPath;

        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Add Cors
            // services.AddCors(options =>
            // {
            //     options.AddPolicy("AllowAllOrigins", builder =>
            //         builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            // });            

            //This method is the one that add the database services.
            ConfigureDatabase(services);

            services.AddMvc();

            services.AddScoped<ICalDav, CalDav>();
            services.AddScoped<IFileManagement, FileManagement>();
            services.AddTransient<IAuthenticate, UhCalendarAuthentication>();
            services.AddScoped<IACLProfind, ACLProfind>();
            services.AddScoped<ICollectionReport, CollectionReport>();
            //services.AddScoped<CalDavContext>();

            //change by yasmany to test

            #region old

            //services.AddScoped<IRepository<CalendarCollection, string>, CollectionRepository>();
            //services.AddScoped<IRepository<CalendarResource, string>, ResourceRespository>();
            //services.AddScoped<IRepository<Principal, string>, PrincipalRepository>();
            //services.AddScoped<IRepository<CalendarHome, string>, CalendarHomeRepository>();
            //services.AddScoped<IPermissionChecker, PermissionsGuard>();
            //services.AddScoped<IReportPreconditions, ReportPreconditions>();

            #endregion

            #region new

            services.AddScoped<ICollectionRepository, CollectionRepository>();
            services.AddScoped<ICalendarResourceRepository, ResourceRespository>();
            services.AddScoped<IPrincipalRepository, PrincipalRepository>();
            services.AddScoped<ICalendarHomeRepository, CalendarHomeRepository>();
            services.AddScoped<IPermissionChecker, PermissionsGuard>();
            services.AddScoped<IReportPreconditions, ReportPreconditions>();

            // add by yasmany (working in contextseed)
            services.AddScoped<CalDavContext>();
            services.AddEntityFrameworkNpgsql().AddDbContext<CalDavContext>();
            services.AddTransient<CalDavContext>();
            services.AddTransient<DbContextSeedData>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. MiddleWares?
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,DbContextSeedData seeder)
        {   //todo:remove after resolve dependency
            //loggerFactory.AddSerilog();

             //app.UseIISPlatformHandler();

            app.UseStaticFiles();

            ConfigureInDev(app, env);          

            // app.UseCors("AllowAllOrigins");            
            //use the authentication middleware
            app.UseAuthorization();

            app.UseMvc();

            seeder.Seed(50);
        }

        /// <summary>
        /// Method for Configure  the system is set into an Development Environment.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public virtual void ConfigureInDev(IApplicationBuilder app, IHostingEnvironment env)
        {
           //Nothing here this is method is used in TestStartup for testing.
        }

        /// <summary>
        /// Method to configure the Database Conections and Settings
        /// </summary>
        /// <param name="services"></param>
        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            #region SQLServer

            // Add framework services.
            services.AddDbContext<CalDavContext>(options =>
            options.UseSqlServer(SystemProperties.SQLServerConnectionString()));
            #endregion

            #region SQLite          

            // services.AddEntityFramework()
            //     .AddSqlite()
            //     .AddDbContext<CalDAVSQLiteContext>(options =>
            //         options.UseSqlite(connection).MigrationsAssembly("DataLayer"));
            //          services.AddDbContext<CalDavContext>(options =>
            //options.UseSqlite(SystemProperties.SQLiteConnectionString()));
            #endregion

            #region Npgsql
            //services.AddDbContext<CalDavContext>(options =>
            //options.UseNpgsql(SystemProperties.NpgsqlConnectionString()));
            #endregion


        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
              .UseKestrel()
              .UseUrls("http://localhost:5003")
              //.UseUrls("http://192.168.99.1:5003")
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseIISIntegration()
              .UseStartup<Startup>()
              .Build();

            host.Run();
        }
        // public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}