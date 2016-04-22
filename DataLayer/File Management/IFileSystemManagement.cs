using System.Collections.Generic;
using System.Threading.Tasks;
using ICalendar.Calendar;

namespace DataLayer
{
    public interface IFileSystemManagement
    {
        /// <summary>
        ///     Get the Root local directory location.
        /// </summary>
        string Root { get; }

        /// <summary>
        ///     Creates a new folder which will contain all user calendar collections.
        /// </summary>
        /// <param name="userEmail">Email of the owner of the folder</param>
        /// <returns></returns>
        bool AddUserFolder(string userEmail);

        /// <summary>
        ///     Creates a new folder which will contain all calendar object resource.
        /// </summary>
        /// <param name="userEmail">Email of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the Calendar Collection</param>
        /// <returns></returns>
        bool AddCalendarCollectionFolder(string userEmail, string calendarCollectionName);

        /// <summary>
        ///     Retrieves all iCalendar Objects stored in calendarCollection specified.
        /// </summary>
        /// <param name="userEmail">Email of the collection owner.</param>
        /// <param name="calendarCollectionName">Name of the Calendar Collection</param>
        /// <param name="calendarObjectResources"></param>
        /// <returns></returns>
        bool GetAllCalendarObjectResource(out Dictionary<string, string> calendarObjectResources);

        /// <summary>
        ///     Returns all iCalendar Objects contained in the collection.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <returns></returns>
        IEnumerable<VCalendar> GetAllCalendarObjectResource();

        /// <summary>
        ///     Deletes the folder which represent the calendar collection and all the COR contained in it.
        /// </summary>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the calendar collection.</param>
        /// <returns></returns>
        bool DeleteCalendarCollection();

        /// <summary>
        ///     Adds a Calendar Object Resource to the calendar collection specified.
        /// </summary>
        /// <param name="objectResourceName">Returns the name assigned to the iCalendar file.</param>
        /// <param name="bodyIcalendar">Body of the iCalendar Object that is going to be stored with .ics extension.</param>
        /// <param name="userEmail">Email of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the calendar collection where is going to be stored</param>
        /// <returns></returns>
        Task<bool> AddCalendarObjectResourceFile(string objectResourceName, string bodyIcalendar);

        /// <summary>
        ///     Returns the iCalendar Object stored in the user calendar collection.
        /// </summary>
        /// <param name="objectResourceName">Name of the iCalendar File (must include .ics extension)</param>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <returns></returns>
        Task<string> GetCalendarObjectResource(string objectResourceName);

        /// <summary>
        ///     Deletes the COR specified
        /// </summary>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <param name="objectResourceName">Name of the iCalendar File (must include .ics extension)</param>
        /// <returns></returns>
        bool DeleteCalendarObjectResource(string objectResourceName);

        /// <summary>
        ///     Check if the folder corresponding with the Calendar Collection Exits
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <returns></returns>
        bool ExistCalendarCollection();

        /// <summary>
        ///     Check if the file corresponding with the Calendar Object Resource exist.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <param name="objectResourceName"></param>
        /// <returns></returns>
        bool ExistCalendarObjectResource(string objectResourceName);

        /// <summary>
        ///     After created the class with the default constructor
        ///     this method set the user and collection where to apply
        ///     the operations.
        /// </summary>
        /// <param name="userId">The desired user.</param>
        /// <param name="collectionId">The desired Collection name.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        bool SetUserAndCollection(string userId, string collectionId);

        /// <summary>
        ///     Create an instance of this class and check if the collection is valid..
        /// </summary>
        /// <param name="userId">The owner of the collection.</param>
        /// <param name="collectionId">The desired collection.</param>
        /// <param name="fileSystemManagement">The instance of the class.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        bool CreateAndCheck(string userId, string collectionId, out IFileSystemManagement fileSystemManagement);

        /// <summary>
        ///     Returns the size of a resource.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        long GetFileSize(string fileName);
    }
}