using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataLayer;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Identity;

namespace ACL.Core
{
    public static class ExtensionMethods
    {
        private static string StreamToString(this Stream stream)
        {
            string output;
            using (var reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }

        /// <summary>
        ///     Take the requested properties for a given principal.
        /// </summary>
        /// <param name="principal">THe principal where to take the properties</param>
        /// <param name="properties">The requested properties.</param>
        /// <returns>The properties with its name and namespace equal to the given.</returns>
        public static IEnumerable<Property> TakeProperties(this Principal principal,
            IEnumerable<KeyValuePair<string, string>> properties)
        {
            return properties
                .Select(pair => principal.Properties
                    .FirstOrDefault(x => x.Name == pair.Key && x.Namespace == pair.Value));
        }

        /// <summary>
        ///     Convert the first letter of the string in UpperCase.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            var a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        ///     Take the names of the properties and convert them
        ///     to the format of our property names.
        /// </summary>
        /// <param name="properties">The properties names to be formated.</param>
        /// <returns>The modified names</returns>
        private static IEnumerable<string> FormatPropertyName(this IEnumerable<string> properties)
        {
            return properties.Select(p =>
            {
                var words = p.Split('-').ToList();
                words.ForEach(x => UppercaseFirst(x));
                var stringB = new StringBuilder();
                foreach (var word in words)
                {
                    stringB.Append(word);
                }
                return stringB.ToString();
            });
        }

        /// <summary>
        ///     Verify if the provided password match with the user's password.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="username">The user email (this is out username)</param>
        /// <param name="password">The provided password.</param>
        /// <returns></returns>
        public static bool VerifyPassword(this CalDavContext context, string username, string password = "")
        {
            var user = context.Users.FirstOrDefault(u => u.Email == username);

            //if the user doenst exit return false
            if (user == null)
                return false;

            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

            //return the result of the password verification
            return result != PasswordVerificationResult.Failed;
        }
    }
}