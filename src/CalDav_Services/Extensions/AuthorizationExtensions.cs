using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDav_Services.Middlewares;
using Microsoft.AspNet.Builder;

namespace CalDav_Services.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}
