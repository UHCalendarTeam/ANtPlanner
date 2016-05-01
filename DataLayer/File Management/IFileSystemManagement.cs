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
       // string Root { get; }

        /// <summary>
        ///     Creates a new folder which will contain all user calendar collections.
        /// </summary>
        /// <param name="principalUrl">Email of the owner of the folder</param>
        /// <returns></returns>
        bool AddPrincipalFolder(string principalUrl);

        /// <summary>
        ///     Creates a new folder which will contain all calendar object resource.
        /// </summary>
        /// <param name="collectionUrl"></param>
        /// <returns></returns>
        bool AddCalendarCollectionFolder(string collectionUrl);

        /// <summary>
        ///     Retrieves all iCalendar Objects stored in calendarCollection specified.
        /// </summary>
        /// <param name="collectionUrl"></param>
        /// <param name="calendarObjectResources"></param>
        /// <param name="userEmail">Email of the collection owner.</param>
        /// <param name="calendarCollectionName">Name of the Calendar Collection</param>
        /// <returns></returns>
        Task<bool> GetAllCalendarObjectResource(string collectionUrl, Dictionary<string, string> calendarObjectResources);

        /// <summary>
        ///     Returns all iCalendar Objects contained in the collection.
        /// </summary>
        /// <param name="collectionUrl"></param>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <returns></returns>
        Task<IEnumerable<VCalendar>> GetAllCalendarObjectResource(string collectionUrl);

        /// <summary>
        ///     Deletes the folder which represent the calendar collection and all the COR contained in it.
        /// </summary>
        /// <param name="collectionUrl"></param>
        /// <returns></returns>
        bool DeleteCalendarCollection(string collectionUrl);

        /// <summary>
        ///     Adds a Calendar Object Resource to the calendar collection specified.
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="body"></param>
        /// <param name="userEmail">Email of the owner of the calendar collection</param>
        /// <param name="calendarCollectionName">Name of the calendar collection where is going to be stored</param>
        /// <returns></returns>
        Task<bool> AddCalendarObjectResourceFile(string resourceUrl, string body);

        /// <summary>
        ///     Returns the iCalendar Object stored in the user calendar collection.
        /// </summary>
        /// <param name="resourceUrl">Name of the iCalendar File (must include .ics extension)</param>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <returns></returns>
        Task<string> GetCalendarObjectResource(string resourceUrl);

        /// <summary>
        ///     Deletes the COR specified
        /// </summary>
        /// <param name="userEmail">Email of the owner of the collection.</param>
        /// <param name="calendarCollectionName">Name of the collection where is stored.</param>
        /// <param name="resourceUrl">Name of the iCalendar File (must include .ics extension)</param>
        /// <returns></returns>
        bool DeleteCalendarObjectResource(string resourceUrl);

        /// <summary>
        ///     Check if the folder corresponding with the Calendar Collection Exits
        /// </summary>
        /// <param name="collectioUrl"></param>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <returns></returns>
        bool ExistCalendarCollection(string collectioUrl);

        /// <summary>
        ///     Check if the file corresponding with the Calendar Object Resource exist.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="calendarCollectionName"></param>
        /// <param name="resourceUrl"></param>
        /// <returns></returns>
        bool ExistCalendarObjectResource(string resourceUrl);

        /// <summary>
        ///     After created the class with the default constructor
        ///     this method set the user and collection where to apply
        ///     the operations.
        /// </summary>
        /// <param name="userId">The desired user.</param>
        /// <param name="collectionId">The desired Collection name.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
       // bool SetUserAndCollection(string userId, string collectionId);

        /// <summary>
        ///     Create an instance of this class and check if the collection is valid..
        /// </summary>
        /// <param name="userId">The owner of the collection.</param>
        /// <param name="collectionId">The desired collection.</param>
        /// <param name="fileSystemManagement">The instance of the class.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        //bool CreateAndCheck(string userId, string collectionId, out IFileSystemManagement fileSystemManagement);

        /// <summary>
        ///     Returns the size of a resource.
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <returns></returns>
        long GetFileSize(string resourceUrl);
    }
}