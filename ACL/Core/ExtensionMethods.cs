
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models.ACL;

namespace ACL.Core
{
    public static class ExtensionMethods
    {
        private static string StreamToString(this Stream stream)
        {
            string output;
            using (StreamReader reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }


        public static string TakeProperties(this Principal principal, IEnumerable<string> properties)
        {
            var formatedNames = properties.FormatPropertyName();
            var propertiesValues = new List<string>();

            foreach (var name in formatedNames)
            {
                
            }

            return "";

        }

        /// <summary>
        /// Convert the first letter of the string in UpperCase.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        /// <summary>
        /// Take the names of the properties and convert them
        /// to the format of our property names.
        /// </summary>
        /// <param name="properties">The properties names to be formated.</param>
        /// <returns>The modified names</returns>
        private static IEnumerable<string>   FormatPropertyName(this IEnumerable<string> properties)
        {
            return  properties.Select(p =>
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
    }
}
