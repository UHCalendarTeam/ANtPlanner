using CalDavServices.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace CalDavServices.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}