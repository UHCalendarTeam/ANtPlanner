using System.IO;
using System.Text;

namespace CalDAV.Core.Method_Extensions
{
    public static class MiscellaneousExtensions
    {
        public static void Write(this Stream stream, string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            stream.Write(data, 0, data.Length);
        }
    }
}