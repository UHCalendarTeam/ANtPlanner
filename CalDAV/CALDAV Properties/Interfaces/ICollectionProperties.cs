using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.CALDAV_Properties
{
    /// <summary>
    /// defines properties for calendar collections
    /// </summary>
    public interface ICollectionProperties
    {
         string NameSpace { get; }

        /// <summary>
        /// Provides a human-readable description of the calendar collection
        /// </summary>
        /// <param name="description">The calendar collection description.</param>
        /// <returns>The XML with description.</returns>
         string CalendarDescription(string userEmail, string collectionName);

        /// <summary>
        /// Purpose: Specifies a time zone on a calendar collection.
        /// Conformance: This property SHOULD be defined on all calendar collections. If defined, it SHOULD NOT
        /// be returned by a PROPFIND DAV:allprop request
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        string CalendarTimeZone(string userEmail, string collectionName);

        /// <summary>
        /// Purpose: This property SHOULD be defined on all calendar collections. If defined, it SHOULD NOT
        /// be returned by a PROPFIND DAV:allprop request.
        /// Conformance: This property SHOULD be defined on all calendar collections. If defined, it SHOULD NOT
        /// be returned by a PROPFIND DAV:allprop request
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        string SupportedCalendarComponentSet(string userEmail, string collectionName);

        /// <summary>
        /// Purpose: Provides a numeric value indicating the maximum size of a resource in octets that the server
        /// is willing to accept when a calendar object resource is stored in a calendar collection.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        string MaxResourcesSize(string userEmail, string collectionName);

        /// <summary>
        /// Purpose: Provides a DATE-TIME value indicating the earliest date and time (in UTC) that the server is
        /// willing to accept for any DATE or DATE-TIME value in a calendar object resource stored in
        ///  a calendar collection.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        string MinDateTime(string userEmail, string collectionName);

        /// <summary>
        /// Purpose: Provides a DATE-TIME value indicating the latest date and time (in UTC) that the server is
        /// willing to accept for any DATE or DATE-TIME value in a calendar object resource stored in
        /// a calendar collection.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        string MaxDateTime(string userEmail, string collectionName);

        /// <summary>
        /// Purpose: Provides a numeric value indicating the maximum number of recurrence instances that a
        /// calendar object resource stored in a calendar collection can generate.
        /// </summary>
        /// <param name="intances"></param>
        /// <returns></returns>
        string MaxIntances(string userEmail, string collectionName);
    }
}
