using System.Net;

namespace Utils
{
    /// <summary>
    ///     THiis class is used for return the response
    ///     of the methods called from the requests.
    /// </summary>
    public class Response
    {
        /// <summary>
        ///     Contains the body of the response.
        /// </summary>
        public string Body { get; set; }

        ///// <summary>
        /////     Contains the response header of the response.
        ///// </summary>
        //public HttpResponseHeader ResponseHeader { get; set; }
    }
}