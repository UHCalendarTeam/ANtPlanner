using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CalDAV.Core;
using CalDAV.Models;
using Microsoft.Data.Entity;

namespace CalDav_Services
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            });

            var connection = @"Server=(localdb)\mssqllocaldb;Database=UHCalendarDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            // Add framework services.
            services.AddEntityFramework()
               .AddSqlServer()
               .AddDbContext<CalDavContext>(options =>
                   options.UseSqlServer(connection).MigrationsAssembly("CalDAV"));

            services.AddMvc();

            services.AddSingleton<ICalDav, CalDav>();
            services.AddSingleton<IFileSystemManagement, FileSystemManagement>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. MiddleWares?
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseCors("AllowAllOrigins");

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
