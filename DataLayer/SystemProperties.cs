using System;
using System.Collections.Generic;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using DataLayer.Models.Repositories;

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
            DeleteCollection,
            DeleteResource,
            ProfindCollection,//profind that is performed on a collection
            ProfindResource,//profind that is performed on a resource
            Report,
            MKCalendar,
            PropatchCollection,
            ProppatchResource
        }

        public enum AuthenticationMethods
        {
            Basic,
            Digest,
            OpenId
        }

        ///Contains the names of the public calendar that are gonna be created
        ///when the admin user is created in the system.
        //
        // Modified this list in order to change the public calendar of the system.
        public static readonly string[] PublicCalendarNames =
        {
            "C111",
            "C112",
            "C113",
            "C121",
            "C122",
            "C123",
            "C211",
            "C212",
            "C213",
            "C311",
            "C312",
            "C411",
            "C412",
            "C511",
            "C512",
            "M111",
            "M112",
            "M211",
            "M212",
            "M311",
            "M411",
            "PublicEvents"
        };

        /// <summary>
        ///     Contains the name of the url that represent the calendar collection
        ///     for the public calendars.
        /// </summary>
        public static string PublicCalendarHomeUrl = "/collections/groups/public/";

        // public static bool PublicCalendarCreated => new FileManagement().ExistCalendarCollection(PublicCalendarHomeUrl);

        /// <summary>
        ///     Check if the admin user is created in the system.
        ///     Till now is used to create the public calendar with the 
        ///     creation of the admin user. 
        /// </summary>
        /// <returns></returns>
        public static bool PublicCalendarCreated
        {
            get
            {
                var principalRepo = new PrincipalRepository(new CalDavContext());
                var admin = principalRepo.FindByIdentifierAsync("admin@admin.uh.cu");
                return admin != null;
            }
        }


        public static AuthenticationMethods AuthenticationMethod => AuthenticationMethods.Basic;

        /// <summary>
        ///     Contains the base url path for the system.
        /// </summary>
        public static readonly string _baseUrl = "";

        /// <summary>
        ///     Contains the url for the user's collections
        ///     Add the email of the user at the end of the path to create the 
        ///     container of an user's calendar
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
        ///     Contains the url that has to be assigned the the pricipal that
        ///     represent a normal user.
        ///     The email of the user has to be added at the end of the url
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


        public static string AbsolutePath { get; set; }


        /// <summary>
        ///     Contains the full namespaces (i.e xmlns:...) that are used in the 
        ///     body of the request from the client and to the client.
        /// </summary>
        public static readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""},
            {"CS", @"xmlns:CS=""http://calendarserver.org/ns/"""}
        };

        /// <summary>
        ///     Contains the value of the namespaces (i.e: DAV:)
        /// </summary>
        public static readonly Dictionary<string, string> NamespacesValues = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"},
            {"CS", @"http://calendarserver.org/ns/"}
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

        #region DB Connection Strings      
        public static string SQLiteConnectionString()
        {
            var path = PlatformServices.Default.Application.ApplicationBasePath;
            var connection = "Filename=" + Path.Combine(path, "UHCalendar.db");
            return connection;
        }

        public static string SQLServerConnectionString()
        {
            return @"Server=(localdb)\mssqllocaldb;Database=UHCalendar;Trusted_Connection=True;MultipleActiveResultSets=True";
            //return @"Server=(localdb)\mssqllocaldb;Database=UHCalendar;Trusted_Connection=True";
        }

        public static string NpgsqlConnectionString()
        {
            return @"User ID=admin;Password=admin;Host=localhost;Port=5432;Database=UHCalendar;Pooling=true;";
        }
        #endregion
    }
}