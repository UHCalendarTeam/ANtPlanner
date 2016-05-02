using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using ACL.Core.Authentication;

namespace CalDav_Services.Middlewares
{
    /// <summary>
    /// This middleware hadles the authorization in the system.
    /// Till now the only authentication schema that the system
    /// supports is Basic.
    /// </summary>
    public class AuthorizationMiddleware:IDisposable
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IAuthenticate _authenticate;

        public AuthorizationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IAuthenticate authenticate)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<AuthorizationMiddleware>();
            _authenticate = authenticate;
        }


        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Handling authorization in resource: " + context.Request.Path);
            var principal =await _authenticate.AuthenticateRequest(context);
            if (principal == null)
            {
                _logger.LogInformation("The client doesn't have enough privileges.");
                return;
            }
            //context.Session.SetString("principalId", principal.PrincipalURL);
            _logger.LogInformation($"Authorization granted for principal: {principal.PrincipalStringIdentifier}");
            await _next.Invoke(context);
            _logger.LogInformation("Finished handling request.");
        }

        public void Dispose()
        {
            _authenticate.Dispose();
        }
    }
}
