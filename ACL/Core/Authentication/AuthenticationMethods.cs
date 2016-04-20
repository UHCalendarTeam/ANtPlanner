using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Extensions.Primitives;


namespace ACL.Core.Authentication
{
    public class AuthenticationMethods : IAuthenticate
    {

        /// <summary>
        /// Takes the necessary content from the UH's authentication API response. 
        /// </summary>
        /// <param name="clientRequest"></param>
        /// <returns></returns>
        public async  Task GetResponseFromRequest(HttpRequest clientRequest)
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


            //var req = (HttpWebRequest) WebRequest.Create("authentication.uh.cu");
            using (var client = new HttpClient())
            {
                ///TODO: check this out!
                client.BaseAddress = new Uri("the_url_base");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ///TODO: check how is done the credential in the UH API
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(username, password);

                // New code:
                HttpResponseMessage response = await client.GetAsync("the_url");

                if (response.IsSuccessStatusCode)
                {
                    //TODO: take the necessary content from here!
                    var content = await response.Content.ReadAsStringAsync();
                }
            }
            //req.Credentials = clientRequest.;
            
        }
    }
}
