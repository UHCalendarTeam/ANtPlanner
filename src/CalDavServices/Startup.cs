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
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using DataLayer.Repositories.Implementations;
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
            services.AddScoped<IRepository<CalendarCollection, string>, CollectionRepository>();
            services.AddScoped<IRepository<CalendarResource, string>, ResourceRespository>();
            services.AddScoped<IRepository<Principal, string>, PrincipalRepository>();
            services.AddScoped<IRepository<CalendarHome, string>, CalendarHomeRepository>();
            services.AddScoped<IPermissionChecker, PermissionsGuard>();
            services.AddScoped<IReportPreconditions, ReportPreconditions>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. MiddleWares?
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            // app.UseIISPlatformHandler();

            //app.UseStaticFiles();


            // app.UseCors("AllowAllOrigins");


            //use the authentication middleware
            app.UseAuthorization();

            app.UseMvc();
        }

        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            //var connection =
            //    @"Server=(localdb)\mssqllocaldb;Database=UHCalendarDB;Trusted_Connection=True;MultipleActiveResultSets=False";
            // Add framework services.
            //services.AddEntityFramework()
            //    .AddSqlServer()
            //    .AddDbContext<CalDavContext>(options =>
            //        options.UseSqlServer(connection).MigrationsAssembly("DataLayer"));

            var path = PlatformServices.Default.Application.ApplicationBasePath;
            var connection = "Filename=" + Path.Combine(path, "UHCalendar.db");

            // services.AddEntityFramework()
            //     .AddSqlite()
            //     .AddDbContext<CalDAVSQLiteContext>(options =>
            //         options.UseSqlite(connection).MigrationsAssembly("DataLayer"));
            services.AddDbContext<CalDAVSQLiteContext>(options =>
  options.UseSqlite(connection));

        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
              .UseKestrel()
              .UseUrls("http://10.6.31.132:5003")
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseIISIntegration()
              .UseStartup<Startup>()
              .Build();

            host.Run();
        }
        // public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}