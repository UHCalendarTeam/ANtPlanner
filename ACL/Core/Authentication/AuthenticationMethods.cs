using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Identity;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Repositories;
using Microsoft.AspNetCore.Http;

namespace ACL.Core.Authentication
{
    public class UhCalendarAuthentication : IAuthenticate
    {
        private readonly IPrincipalRepository _principalRepository;

        /// <summary>
        ///     Injects an instance of CaldavContext
        /// </summary>
        /// <param name="context"></param>
        public UhCalendarAuthentication(IPrincipalRepository principalRepository)
        {
            _principalRepository = principalRepository as PrincipalRepository;
        }


        /// <summary>
        ///     Takes the necessary content from the UH's authentication API response.
        ///     Check if the user exist in the system, if does then check if the authentication
        ///     credential are OK.
        ///     If dont then take the user data from UH apis and create the user in the
        ///     system with
        /// 
        /// 
        /// 
        ///  this data.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<Principal> AuthenticateRequestAsync(HttpContext httpContext)
        {
            var username = "";
            Principal principal = null;
            string cookieValue;

            //take the creadentials from the request
            string authHeader = httpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                var credentials = TakeCredentials(authHeader);
                username = credentials.Key;
                var password = credentials.Value;
                principal = _principalRepository.FindByIdentifier(username);
                //check if the user exist in our DB
                if (principal != null)
                {
                    // if does then check if can authenticate
                    //if the username and password doesnt match then return 401 - Unauthorized
                    if (!_principalRepository.VerifyPassword(principal, password))
                    {
                        SetUnauthorizedRequest(httpContext);
                        return null;
                    }
                }



                //Temporaly if the WCF services doesnt work we are gonna create
                // the users automatically in the system.
                // TODO: check if is a student or teacher

                //if the user is new in our system then create him
                //TODO: change this if dont want the new user automatic creation behavior
                else
                {
//                    principal = _principalRepository.CreateUserInSystem(username, username, password);
//
//                    Console.WriteLine($"------Created user with username: {username}");
                }


                //TODO: change to this when work the WCF service
                //var userData = GetUserDataFromUhApi(username);
            }

            if (principal != null)
                return principal;

            #region checking cookies

            //if the request doesn't have an Authorization header then
            //ckeck the session cookies.
            //else
            //{
            //    //if the request doens't comes with a authorization header
            //    // then check if has the cookie provided by us
            //    // 
            //    if (!httpContext.Request.Cookies.ContainsKey(SystemProperties._cookieSessionName))
            //    {
            //        /*
            //        |   if the request neither contains the session cookie nor the
            //        |  Authorization header then the client needs to request
            //        |   the credential to the user. So send a 401
            //        */
            //        await SetUnauthorizedRequest(httpContext);
            //        return null;
            //    }
            //    //take the cookie that the client send us in the request
            //    cookieValue = httpContext.Request.Cookies[SystemProperties._cookieSessionName];

            //    principal =await _principalRepository.GetByCookie(cookieValue);
            //    if(principal == null)
            //    {
            //        await SetUnauthorizedRequest(httpContext);
            //        return null;
            //    }

            //}


            //set the cookie for the response.
            //cookieValue = Guid.NewGuid().ToString();
            //httpContext.Response.Cookies.Append(SystemProperties._cookieSessionName, cookieValue);
            //await _principalRepository.SetCookie(username, cookieValue);

            #endregion

            SetUnauthorizedRequest(httpContext);
            return null;
            //return await Task.FromResult(principal);
        }

        /// <summary>
        ///     Takes the necessary content from the UH's authentication API response.
        ///     Check if the user exist in the system, if does then check if the authentication
        ///     credential are OK.
        ///     If dont then take the user data from UH apis and create the user in the
        ///     system with this data.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Principal AuthenticateRequest(HttpContext httpContext)
        {
            Principal principal = null;
            string cookieValue;

            //take the creadentials from the request
            string authHeader = httpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                var credentials = TakeCredentials(authHeader);
                var username = credentials.Key;
                var password = credentials.Value;
                principal = _principalRepository.FindByIdentifier(username);
                //check if the user exist in our DB
                if (principal != null)
                {
                    // if does then check if can authenticate
                    //if the username and password doesnt match then return 401 - Unauthorized
                    if (!_principalRepository.VerifyPassword(principal, password))
                    {
                        SetUnauthorizedRequest(httpContext);
                        return null;
                    }
                }
                else
                {
//                    principal = _principalRepository.CreateUserInSystem(username, username, password);
//
//                    Console.WriteLine($"------Created user with username: {username}");
                }

                //TODO: change to this when work the WCF service
                //var userData = GetUserDataFromUhApi(username);
            }

            if (principal != null)
            {
                return principal;
            }

            #region checking cookies

            //if the request doesn't have an Authorization header then
            //ckeck the session cookies.
            //else
            //{
            //    //if the request doens't comes with a authorization header
            //    // then check if has the cookie provided by us
            //    // 
            //    if (!httpContext.Request.Cookies.ContainsKey(SystemProperties._cookieSessionName))
            //    {
            //        /*
            //        |   if the request neither contains the session cookie nor the
            //        |  Authorization header then the client needs to request
            //        |   the credential to the user. So send a 401
            //        */
            //        await SetUnauthorizedRequest(httpContext);
            //        return null;
            //    }
            //    //take the cookie that the client send us in the request
            //    cookieValue = httpContext.Request.Cookies[SystemProperties._cookieSessionName];

            //    principal =await _principalRepository.GetByCookie(cookieValue);
            //    if(principal == null)
            //    {
            //        await SetUnauthorizedRequest(httpContext);
            //        return null;
            //    }

            //}


            //set the cookie for the response.
            //cookieValue = Guid.NewGuid().ToString();
            //httpContext.Response.Cookies.Append(SystemProperties._cookieSessionName, cookieValue);
            //await _principalRepository.SetCookie(username, cookieValue);

            #endregion

            SetUnauthorizedRequest(httpContext);
            return null;
            //return await Task.FromResult(principal);
        }

        public KeyValuePair<string, string> TakeCredentials(HttpContext context)
        {
            //take the creadentials from the request
            string authHeader = context.Request.Headers["Authorization"];
            

            if (!string.IsNullOrEmpty(authHeader))
            {
                var credentials = TakeCredentials(authHeader);

                return credentials;
            }
            return new KeyValuePair<string, string>(null, null);
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
            //TODO: take the useful data from the content
            //check if the user is in the system
            // if not create it with all his stuffs
            // send the data back
            //put the data in the output

            //create the token and shit

            #endregion taking data from the UH api
        }

        #region Methods for taking credential from the authorization header

        /// <summary>
        ///     Take the user credential from the Authorization header.
        ///     Checks if the credential is basic or digest.
        /// </summary>
        /// <param name="authHeader">The header with the credentials</param>
        /// <returns></returns>
        public KeyValuePair<string, string> TakeCredentials(string authHeader)
        {
            var credentials = authHeader.StartsWith("Basic")
                ? TakeCredentialsFromBasic(authHeader)
                : TakeCredentialsFromDigest(authHeader);
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
        private KeyValuePair<string, string> TakeCredentialsFromBasic(string basicAuthorizationString)
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
        ///     Take the credential from the Digest authentication
        ///     that comes in the request from the client.
        /// </summary>
        /// <returns>The username and password</returns>
        //private KeyValuePair<string, string> TakeCredentionFromDigest(string digestAuthHeader)
        //{
        //    throw new NotImplementedException("The Digest Authentication method is not supported yet.");
        //}
        /// <summary>
        ///     Create the nonce for the client and set the
        ///     authentication header of the response specifying
        ///     the Digest authentication and sending the nonce.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        //private void GenerateDigestHeader(HttpContext httpContext)
        //{
        //    throw new NotImplementedException("The Digest Authentication method is not supported yet.");
        //}
        /// <summary>
        ///     Set in the response the 401 - Unauthorized
        ///     and add a message to the response body.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private void SetUnauthorizedRequest(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            switch (SystemProperties.AuthenticationMethod)
            {
                case SystemProperties.AuthenticationMethods.Basic:
                    httpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{httpContext.Request.Path}\"";
                    break;
                case SystemProperties.AuthenticationMethods.Digest:
                    GenerateDigestHeader(httpContext);
                    break;
            }
        }

        private void GenerateDigestHeader(HttpContext httpContext)
        {
            throw new NotImplementedException("Digest Authentication is not supported yet.");
        }

        private KeyValuePair<string, string> TakeCredentialsFromDigest(string digestAuthHeader)
        {
            throw new NotImplementedException("Digest Authentication is not supported yet.");
        }

        #endregion
    }
}