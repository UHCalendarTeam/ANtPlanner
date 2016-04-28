using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.ExtensionMethods;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace ACL.Core.Authentication
{
    public class UhCalendarAuthentication : IAuthenticate
    {
        private readonly CalDavContext _context;

        /// <summary>
        ///     Injects an instance of CaldavContext
        /// </summary>
        /// <param name="context"></param>
        public UhCalendarAuthentication(CalDavContext context)
        {
            _context = context;
        }


        /// <summary>
        ///     Takes the necessary content from the UH's authentication API response.
        ///     Check if the user exist in the system, if does then check if the authentication
        ///     credential are OK.
        ///     If dont then take the user data from UH apis and create the user in the
        ///     system with this data.
        /// </summary>
        /// <param name="clientRequest">
        ///     THe request from the client. THe authentication
        ///     credential are taken from it.
        /// </param>
        /// <returns></returns>
        public async Task<Principal> AuthenticateRequest(HttpContext httpContext)
        {
            var username = "";
            var password = "";
            var authorizationGranted = false;

            ///take the creadentials from the request

            #region take the authorization header and proccess it

            string authHeader = httpContext.Request.Headers["Authorization"];
            ///check if has the authorization header and is basic
            if (!string.IsNullOrEmpty(authHeader))
            {
                var credentials = TakeCreadential(authHeader).Result;
                username = credentials.Key;
                password = credentials.Value;

                ///check if the user exist in our DB
                if (_context.Principals.Any(p => p.PrincipalStringIdentifier == username))
                {
                    /// if does then check if can authenticate
                    if (_context.VerifyPassword(username, password))
                    {
                        Console.WriteLine($"------Current user {username} is authenticated");
                    }

                    //if the username and password doesnt match then return 401 - Unauthorized
                    else
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return null;
                    }
                }

                //if the user is new in our system then create him
                //TODO: change this if dont want the new user automatic creation behavior
                else
                {
                    ///Temporaly if the WCF services doesnt work we are gonna create
                    /// the users automatically in the system.
                    /// TODO: check if is a student or teacher

                    _context.CreateUserInSystem(username, username, password);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"------Created user with username: {username}");

                    //TODO: change to this when work the WCF service
                    //var userData = GetUserDataFromUhApi(username);
                }
            }
            //if the request doesn't have an Authorization header then
            //ckeck the session cookies.
            else
            {
                ///if the request doens't comes with a authorization header
                /// then check if has the cookie provided by us
                /// 
                if (!httpContext.Request.Cookies.ContainsKey(SystemProperties._cookieSessionName))
                {
                    /*
                    |   if the request neither contains the session cookie nor the
                    |  Authorization header then the client needs to request
                    |   the credential to the user. So send a 401
                    */
                    await SetUnauthorizedRequest(httpContext);
                    return null;
                }
                //take the cookie that the client send us in the request
                var cookieValue = httpContext.Request.Cookies[SystemProperties._cookieSessionName];

                var principalStringId = httpContext.Session.GetString("principalId");
                //if the session doesnt have the principalId means somethind is wrong
                if (string.IsNullOrEmpty(principalStringId))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return null;
                }

                //take the principal from our system
                var principal =
                    _context.Principals.FirstOrDefaultAsync(p => p.PrincipalStringIdentifier == principalStringId)
                        .Result;
                //check if the cookies match
                var principalSesison = principal.SessionId;
                if (!VerifySessionCookies(principalSesison, cookieValue))
                {
                    ///if the cookies doesnt match then send back 401
                    await SetUnauthorizedRequest(httpContext);
                    return null;
                }
            }

            #endregion

            return
                await
                    _context.Principals.Include(p => p.Properties)
                        .FirstOrDefaultAsync(x => x.PrincipalStringIdentifier == username);
        }


        /// <summary>
        ///     Verifies if the client session cookie match with the one
        ///     that it says to represent.
        ///     Now the verification is done comparing the string
        ///     TODO: Hash the session cookies.
        /// </summary>
        /// <param name="clientSesison">The string cookie session from the client</param>
        /// <param name="principalSession"></param>
        /// <returns></returns>
        private bool VerifySessionCookies(string clientSesison, string principalSession)
        {
            if (string.IsNullOrEmpty(clientSesison) || string.IsNullOrEmpty(principalSession))
                return false;

            return clientSesison == principalSession;
        }

        /// <summary>
        ///     Takes the data for the new user in the system
        ///     from the Uh directory API.
        ///     Has a problem adding the service to the project.
        /// </summary>
        /// <param name="userEmail">The new user email that's gonna be created.</param>
        /// <returns>The user data.</returns>
        private Task<Dictionary<string, string>> GetUserDataFromUhApi(string userEmail)
        {
            throw new NotImplementedException("Not implemented yet because the communication with the API");

            #region taking data from the UH api

            /*

     //Igore the invalid certificate
    ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;

    //TODO: change this line to the name of the service
    ///create the service for the communication with the SOAP service
    var service = new ServiceReference1.DataServiceSoapClient("DataServiceSoap");
    ///TODO: check if the user can authenticate
     var response = service.Autentificarse(username, password);
    //take the data from the service
    var dd = service.DatosEstudiante(response, FiltrosEstudiante.Docentes);

    //take the student career
    var career = dd.docentData.career;

    //take the student's group
    var group = dd.docentData.group;

    //take the year
    var year = dd.docentData.year;

    //call the user(student) creation method in the DataLayer
    */
            ///TODO: take the useful data from the content
            ///check if the user is in the system
            /// if not create it with all his stuffs
            /// send the data back
            ///put the data in the output

            //create the token and shit

            #endregion taking data from the UH api
        }

        #region Methods for taking credential from the authorization header

        /// <summary>
        ///     Take the user credential from the Authorization header.
        ///     Checks if the credential are from basic or digest.
        /// </summary>
        /// <param name="authHeader">The header with the credentials</param>
        /// <returns></returns>
        public async Task<KeyValuePair<string, string>> TakeCreadential(string authHeader)
        {
            KeyValuePair<string, string> credentials;
            if (authHeader.StartsWith("Basic"))
            {
                credentials = TakeCredentionFromBasic(authHeader).Result;
            }
            else
            {
                //Handle what happens if that isn't the case
                credentials = TakeCredentionFromDigest(authHeader).Result;
            }
            return credentials;
        }


        /// <summary>
        ///     Decrypts the username and password that comes in the
        ///     Authorization header
        /// </summary>
        /// <param name="basicAuthorizationString">
        ///     The string that cotnains the basic
        ///     authorization credentials.
        /// </param>
        /// <returns>A KeyValuePair that contains the username and password</returns>
        private async Task<KeyValuePair<string, string>> TakeCredentionFromBasic(string basicAuthorizationString)
        {
            var encodedUsernamePassword = basicAuthorizationString.Substring("Basic ".Length).Trim();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);
            return new KeyValuePair<string, string>(username, password);
        }

        /// <summary>
        ///     Take the credential from the Digest authorization
        ///     that comes in the request from the client.
        /// </summary>
        /// <param name="digestAuthorizationString"></param>
        /// <returns>The username and password</returns>
        private async Task<KeyValuePair<string, string>> TakeCredentionFromDigest(string digestAuthorizationString)
        {
            //TODO: implement this
            throw new NotImplementedException("The Digest Authorization is not supported in UhCalendar system.");
        }

        /// <summary>
        ///     Set in the response the 401 - Unauthorized
        ///     and add a message to the response body.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task SetUnauthorizedRequest(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //TODO: write something in the body
            await Task.FromResult(0);
        }

        #endregion
    }
}