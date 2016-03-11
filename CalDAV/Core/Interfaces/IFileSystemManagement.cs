using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    public interface IFileSystemManagement
    {
        /// <summary>
        /// Get the Root local directory location.
        /// </summary>
        string Root { get; }
        /// <summary>
        /// Creates a new folder which will contain all user calendar collections.
        /// </summary>
        /// <param name="userName">Name of the owner of the folder</param>
        /// <returns></returns>
        bool AddUserFolder(string userName);
        /// <summary>
        /// Creates a new folder which will contain all calendar object resource.
        /// </summary>
        /// <param name="userName">Name of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the Calendar Collection</param>
        /// <returns></returns>
        bool AddCalendarFolder(string userName, string calendarCollectionName);
        /// <summary>
        /// Adds a Calendar Object Resource to the calendar collection specified. 
        /// </summary>
        /// <param name="userName">Name of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the calendar collection where is going to be stored</param>
        /// <param name="bodyIcalendar">Body of the iCalendar Object that is going to be stored with .ics extension.</param>
        /// <param name="objectResourceName">Returns the name assigned to the iCalendar file.</param>
        /// <returns></returns>
        bool AddCalendarObjectResourceFile(string userName, string calendarCollectionName, string bodyIcalendar, out string objectResourceName);
        /// <summary>
        /// Returns the iCalendar Object stored in the user calendar collection.
        /// </summary>
        /// <param name="userName">Name of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <param name="objectResourceName">Name of the iCalendar File (must include .ics extension)</param>
        /// <returns></returns>
        string GetCalendarObjectResource(string userName, string calendarCollectionName, string objectResourceName);
    }
}
