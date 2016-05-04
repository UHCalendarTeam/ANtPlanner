using ACL.Core;
using ACL.Core.Authentication;
using CalDavServices.Extensions;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CalDAV.Core;
using DataLayer;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.Data.Entity;



namespace CalDavServices
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

            var connection = @"Server=(localdb)\mssqllocaldb;Database=UHCalendarDB;Trusted_Connection=True;MultipleActiveResultSets=False";
            // Add framework services.
            services.AddEntityFramework()
               .AddSqlServer()
               .AddDbContext<CalDavContext>(options =>
                   options.UseSqlServer(connection).MigrationsAssembly("DataLayer"));
            
            services.AddMvc();

            

            

            services.AddScoped<ICalDav, CalDav>();
            services.AddScoped<IFileSystemManagement, FileSystemManagement>();
            services.AddTransient<IAuthenticate, UhCalendarAuthentication>();
            services.AddScoped<IACLProfind, ACLProfind>();
            services.AddScoped<ICollectionReport, CollectionReport>();
            //services.AddScoped<CalDavContext>();
            services.AddScoped<IRepository<CalendarCollection, string>, CollectionRepository>();
            services.AddScoped<IRepository<CalendarResource, string>, ResourceRespository>();
            services.AddScoped<IRepository<Principal, string>, PrincipalRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. MiddleWares?
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Warning;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

           

           
            app.UseIISPlatformHandler();

            //app.UseStaticFiles();

            

            //app.UseCors("AllowAllOrigins");


            //use the authentication middleware
            app.UseAuthorization();

            app.UseMvc();

            
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
