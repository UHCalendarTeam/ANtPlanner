using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Extensions.Primitives;


namespace ACL.Core.Authentication
{
    public class AuthenticationMethods 
    {

        /// <summary>
        /// Takes the necessary content from the UH's authentication API response. 
        /// </summary>
        /// <param name="clientRequest"></param>
        /// <returns></returns>
        public async  Task<Dictionary<string, string>> AuthenticateRequest(HttpRequest clientRequest)
        {
            string username;
            string password;
            

            ///take the creadentials from the request
            string  authHeader = clientRequest.Headers["Authorization"];

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
            else {
                //Handle what happens if that isn't the case
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }

            ///check if the user exist in out DB
            /// if does then check if can authenticate
            if (AuthenticateInSystem(username, password))
            {
            }

            else
            {
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
            }

            //create the token and shit

            var output = new Dictionary<string, string>();

            return output;
        }


        public bool AuthenticateInSystem(string useremail, string password)
        {
            var context = new CalDavContext();
            
            //take the user with the gieven username
            var user = context.Users.FirstOrDefault(u => u.Email == useremail);

            //if null the user doesn't exist
            if (user == null)
                return false;

            //TODO: check the user password and see if match the password
            return true;
        }
    }
}
