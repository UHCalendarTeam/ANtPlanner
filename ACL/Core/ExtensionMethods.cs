
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
