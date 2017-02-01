using System.Threading.Tasks;
using ACL.Core.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CalDavServices.Middlewares
{
    /// <summary>
    ///     This middleware hadles the authorization in the system.
    ///     Till now the only authentication schema that the system
    ///     supports is Basic.
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly IAuthenticate _authenticate;
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IAuthenticate authenticate)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<AuthorizationMiddleware>();
            _authenticate = authenticate;
        }


        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Handling authorization in resource: " + context.Request.Path);
            var principal = _authenticate.AuthenticateRequest(context);

            if (principal == null)
            {
                _logger.LogInformation("The client doesn't have enough privileges.");
            }
            else
            {
                //context.Session.SetString("principalId", principal.PrincipalURL);
                _logger.LogInformation($"Authorization granted for principal: {principal.PrincipalStringIdentifier}");
                await _next.Invoke(context);
                _logger.LogInformation("Finished handling request.");
            }
        }
    }
}