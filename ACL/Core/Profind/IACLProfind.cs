using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;

namespace ACL.Core
{
    public interface IACLProfind
    {
        Task Profind(HttpRequest request, HttpResponse response);

        Task BuildResponse(HttpResponse response, string requestedUrl,
            List<KeyValuePair<string, string>> reqProperties, Principal principal);

    }
}