using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace CalDavServices
{
    /// <summary>
    /// This class is used for test porpoise only.
    /// </summary>
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        public override void ConfigureDatabase(IServiceCollection services)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            services
              .AddEntityFrameworkSqlite()
              .AddDbContext<CalDAVSQLiteContext>(
                options => options.UseSqlite(connection)
              );
        }
    }
}
