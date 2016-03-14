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
        /// <param name="userEmail">Email of the owner of the folder</param>
        /// <returns></returns>
        bool AddUserFolder(string userEmail);
        /// <summary>
        /// Creates a new folder which will contain all calendar object resource.
        /// </summary>
        /// <param name="userEmail">Email of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the Calendar Collection</param>
        /// <returns></returns>
        bool AddCalendarCollectionFolder(string userEmail, string calendarCollectionName);

        /// <summary>
        /// Retrieves all iCalendar Objects stored in calendarCollection specified.
        /// </summary>
        /// <param name="userEmail">Email of the collection owner.</param>
        /// <param name="calendarCollectionName">Name of the Calendar Collection</param>
        /// <param name="calendarObjectResources"></param>
        /// <returns></returns>
        bool GetAllCalendarObjectResource(string userEmail, string calendarCollectionName,out List<string> calendarObjectResources );
        /// <summary>
        /// Deletes the folder which represent the calendar collection and all the COR contained in it.
        /// </summary>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the calendar collection.</param>
        /// <returns></returns>
        bool DeleteCalendarCollection(string userEmail, string calendarCollectionName);

        /// <summary>
        /// Adds a Calendar Object Resource to the calendar collection specified. 
        /// </summary>
        /// <param name="userEmail">Email of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the calendar collection where is going to be stored</param>
        /// <param name="objectResourceName">Returns the name assigned to the iCalendar file.</param>
        /// <param name="bodyIcalendar">Body of the iCalendar Object that is going to be stored with .ics extension.</param>
        /// <returns></returns>
        bool AddCalendarObjectResourceFile(string userEmail, string calendarCollectionName, string objectResourceName, string bodyIcalendar);
        /// <summary>
        /// Returns the iCalendar Object stored in the user calendar collection.
        /// </summary>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <param name="objectResourceName">Name of the iCalendar File (must include .ics extension)</param>
        /// <returns></returns>
        string GetCalendarObjectResource(string userEmail, string calendarCollectionName, string objectResourceName);
        /// <summary>
        /// Deletes the COR specified
        /// </summary>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <param name="objectResourceName">Name of the iCalendar File (must include .ics extension)</param>
        /// <returns></returns>
        bool DeleteCalendarObjectResource(string userEmail, string calendarCollectionName, string objectResourceName);
        /// <summary>
        /// Check if the folder corresponding with the Calendar Collection Exits
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <returns></returns>
        bool ExistCalendarCollection(string userEmail, string calendarCollectionName);
        /// <summary>
        /// Check if the file corresponding with the Calendar Object Resource exist.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <param name="objectResourceName"></param>
        /// <returns></returns>
        bool ExistCalendarObjectResource(string userEmail, string calendarCollectionName,
            string objectResourceName);
    }
}
