using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.ExtensionMethods;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Primitives;


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
        public async Task<Principal> AuthenticateRequest(HttpRequest clientRequest, HttpResponse response)
        {
            string username;
            string password;
            var context = new CalDavContext();


            ///take the creadentials from the request
            string authHeader = clientRequest.Headers["Authorization"];

            ///check if has the authorization header and is basic
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                username = usernamePassword.Substring(0, seperatorIndex);
                password = usernamePassword.Substring(seperatorIndex + 1);
            }
            else
            {
                //Handle what happens if that isn't the case
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }

            ///check if the user exist in out DB
            /// if does then check if can authenticate
            if (context.VerifyPassword(username, password))
            {
               
            }


            else
            {
                ///Temporaly if the WCF services doesnt work we are gonna create
                /// the users automatically in the system.
                /// TODO: check if is a student or teacher
                context.CreateUserInSystem(username, "Defaul User", password);
                context.SaveChanges();
              

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

                #endregion
            }

            response.Cookies.Append("AuthId", Guid.NewGuid().ToString());
            //take the user with the email and take the principal that
            //represents him.
            var prinId = context.Users.FirstOrDefault(x => x.Email == username).PrincipalId;
            return await context.Principals.Include(p=>p.Properties).FirstOrDefaultAsync(x=>x.PrincipalId == prinId);
        }
    }

    
}
