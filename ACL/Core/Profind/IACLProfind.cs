using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ACL.Core
{
    public interface IACLProfind
    {
        Task Profind(HttpRequest request, HttpResponse response, Dictionary<string, string> data);

        Task BuildResponse(HttpResponse response, string requestedUrl,
            List<KeyValuePair<string, string>> reqProperties, Principal principal);
    }
}