using System;
using System.Collections.Generic;

namespace DataLayer
{
    /// <summary>
    ///     Contains some useful properties for the system.
    ///     Declare here the appSettings.
    /// </summary>
    public static class SystemProperties
    {
        /// <summary>
        ///     A principal represent either a group or
        ///     a user.
        /// </summary>
        public enum PrincipalType
        {
            Group,
            Student,
            Worker,
            User
        }

        public enum HttpMethod
        {
           
            PutCreate,//the put that create resources
            PutUpdate,//the put that modified resources
            Get,
            Delete,
            ProfindCollection,//profind that is performed on a collection
            ProfindResource,//profind that is performed on a resource
            Report,
            MKCalendar,
            PropatchCollection,
            ProppatchResource
        }

        public static readonly string[] PublicCalendarNames =
        {
            "1stYearCom",
            "2ndYearCom",
            "3rdYearCom",
            "4thYearCom",
            "5thYearCom",
            "1stYearMat",
            "2ndYearMat",
            "3rdYearMat",
            "4thYearMat",
            "PublicCalendar"
        };

        public static string PublicCalendarHomeUrl = "collections/groups/public/";

        

        /// <summary>
        ///     Contains the base url path for the system.
        /// </summary>
        public static readonly string _baseUrl = "";

        /// <summary>
        ///     Contains the url for the user's collections
        ///     Add the email of the user
        /// </summary>
        public static readonly string _userCollectionUrl = "/collections/users/";

        /// <summary>
        ///     Contains the url for the groups collection.
        ///     Add the name of the group
        /// </summary>
        public static readonly string _groupCollectionUrl = "/collections/groups/";

        /// <summary>
        ///     Contains the default name for the user collections
        /// </summary>
        public static readonly string _defualtInitialCollectionName = "DefaultCalendar";

        public static readonly string _principalUrl = "principals";

        /// <summary>
        ///     Contains the url that has to be assigned the the priciapl that
        ///     represent a normal user.
        ///     The email of the user has to be added to the end of the url
        /// </summary>
        public static readonly string _userPrincipalUrl = "/principals/users/";

        /// <summary>
        ///     Contains the url that has to be assigned the the priciapl that
        ///     represent a group.
        ///     The name of the group has to be added.
        /// </summary>
        public static readonly string _groupPrincipalUrl = "/principals/groups/";

        /// <summary>
        ///     Contains the name of the cookie that is send for the validation
        ///     in the system of a client.
        /// </summary>
        public static readonly string _cookieSessionName = "authSession";


        /// <summary>
        ///     Contains the full ns (i.e xmlns:...)
        /// </summary>
        public static readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""},
            {"S", @"xmlns:S=""http://calendarserver.org/ns/"""}
        };

        /// <summary>
        ///     Contains the value of the ns (i.e: DAV:)
        /// </summary>
        public static readonly Dictionary<string, string> NamespacesValues = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"},
            {"S", @"http://calendarserver.org/ns/"}
        };

        /// <summary>
        ///     Build the url that identify the collection where are the
        ///     calendar of principal.
        /// </summary>
        /// <param name="type">A principal can represent either a user or group.</param>
        /// <param name="principalId">
        ///     If the principal represents a user then put ith email here
        ///     otherwise put the name of the group here.
        /// </param>
        /// <returns>The url where to find the calendars of the principal.</returns>
        public static string BuildHomeSetUrl(PrincipalType type, string principalId)
        {
            //take the beginning of the url depending of the king of principal
            var colUrl = type == PrincipalType.User ? _userCollectionUrl : _groupCollectionUrl;

            //add the identifier of the pricipal
            return $"{_baseUrl}{colUrl}{principalId}/";
        }

        //This two methods will give me a month up and down from NOW
        public static string MinDateTime()
        {
            var thisMonth = DateTime.Now.Month;
            var thisDay = DateTime.Now.Day;
            return
                new DateTime(DateTime.Now.Year, thisMonth - 1 == 0 ? 12 : thisMonth - 1, thisDay > 28 ? 28 : thisDay)
                    .ToUniversalTime()
                    .ToString("yyyyMMddTHHmmssZ");
        }

        public static string MaxDateTime()
        {
            var thisMonth = DateTime.Now.Month;
            var thisDay = DateTime.Now.Day;
            return
                new DateTime(DateTime.Now.Year, thisMonth + 1 == 13 ? 1 : thisMonth + 1, thisDay > 28 ? 28 : thisDay)
                    .ToUniversalTime()
                    .ToString("yyyyMMddTHHmmssZ");
        }
    }
}