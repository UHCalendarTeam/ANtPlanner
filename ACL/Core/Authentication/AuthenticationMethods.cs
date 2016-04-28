using DataLayer;
using DataLayer.ExtensionMethods;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACL.Core.Authentication
{
    public class UhCalendarAuthentication : IAuthenticate
    {
        /// <summary>
        /// Takes the necessary content from the UH's authentication API response.
        /// Check if the user exist in the system, if does then check if the authentication
        /// credential are OK.
        /// If dont then take the user data from UH apis and create the user in the
        /// system with this data.
        /// </summary>
        /// <param name="clientRequest">THe request from the client. THe authentication
        /// credential are taken from it.</param>
        /// <returns></returns>
        public async Task<Principal> AuthenticateRequest(HttpContext httpContext)
        {
            string username = "";
            string password = "";
            var context = new CalDavContext();

            ///take the creadentials from the request


            #region take the authorization header and proccess it

            string authHeader = httpContext.Request.Headers["Authorization"];
            ///check if has the authorization header and is basic
            if (authHeader != null)
            {
                KeyValuePair<string, string> credentials;
                //till now we just accept basic authentication
                //TODO: implement DIGET authentication
                if (authHeader.StartsWith("Basic"))
                {
                    credentials = TakeCredentionFromBasic(authHeader).Result;
                }
                else
                {
                    //Handle what happens if that isn't the case
                    credentials = TakeCredentionFromDigest(authHeader).Result;
                }
            }
            else
            {
                ///if the request doens't comes with a authorization header
                /// then check if has the cookie provided by us
                /// If the request comes with a session cookie means
                /// that sends the principalStringId in the url
                var cookieValue = httpContext.Request.Cookies["UhCalendarSession"];

                
            }

            #endregion

            ///check if the user exist in out DB
            /// if does then check if can authenticate
            if (context.VerifyPassword(username, password))
            {
                Console.WriteLine($"------Current user {username} is authenticated");
                httpContext.Response.Cookies.Append("AuthId", Guid.NewGuid().ToString());
            }
            else
            {
                ///Temporaly if the WCF services doesnt work we are gonna create
                /// the users automatically in the system.
                /// TODO: check if is a student or teacher
                context.CreateUserInSystem(username, "Defaul User", password);
                await context.SaveChangesAsync();
                Console.WriteLine($"------Created user with username: {username}");

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


            //take the user with the email and take the principal that
            //represents him.
            var prinId = context.Users.FirstOrDefault(x => x.Email == username).PrincipalId;
            return await context.Principals.Include(p => p.Properties).FirstOrDefaultAsync(x => x.PrincipalId == prinId);
        }


        #region Methods for taking credential from the authorization header

        /// <summary>
        ///Decrypts the username and password that comes in the
        ///Authorization header
        /// </summary>
        /// <param name="basicAuthorizationString">The string that cotnains the basic
        /// authorization credentials.</param>
        /// <returns>A KeyValuePair that contains the username and password</returns>
        private async Task<KeyValuePair<string, string>> TakeCredentionFromBasic(string basicAuthorizationString)
        {

            string encodedUsernamePassword = basicAuthorizationString.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);
            return new KeyValuePair<string, string>(username, password);
        }

        /// <summary>
        /// Take the credential from the Digest authorization
        /// that comes in the request from the client.
        /// </summary>
        /// <param name="digestAuthorizationString"></param>
        /// <returns>The username and password</returns>
        private async Task<KeyValuePair<string, string>> TakeCredentionFromDigest(string digestAuthorizationString)
        {
            //TODO: implement this
            throw new NotImplementedException("The Digest Authorization is not supported in UhCalendar system.");
        }

        #endregion

        
    }
}