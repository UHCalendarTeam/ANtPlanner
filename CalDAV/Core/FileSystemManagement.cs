using System;
using System.Collections.Generic;
using System.IO;
using ICalendar.Calendar;

namespace CalDAV.Core
{
    public class FileSystemManagement : IFileSystemManagement
    {
        /// <summary>
        ///     Use this constructor to set the root path of the files.
        /// </summary>
        /// <param name="root"></param>
        public FileSystemManagement(string root = "\\CalDav\\Users")
        {
            if (root != "\\CalDav\\Users" && !string.IsNullOrEmpty(root) &&
                Uri.IsWellFormedUriString(root, UriKind.Relative) && Path.IsPathRooted(root))
                Root = root;
            else
                Root = Directory.GetCurrentDirectory() + root;
        }

        public FileSystemManagement(string userId, string collectionId, string root = "\\CalDav\\Users")
        {
            Root = root;
            UserId = userId;
            CollectionId = collectionId;
            CollectionPath = Path.GetFullPath(Root) + "\\" + userId + "\\Calendars\\" + collectionId;
        }

        /// <summary>
        ///     Contains the userId where to apply the operations.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Contains the collection name where to apply the operations.
        /// </summary>
        public string CollectionId { get; set; }

        /// <summary>
        ///     Contains the path to the collection where to apply the operations..
        /// </summary>
        public string CollectionPath { get; set; }

        /// <summary>
        ///     Contains the root where are the collections.
        /// </summary>
        public string Root { get; }

        /// <summary>
        ///     After created the class with the default constructor
        ///     this method set the user and collection where to apply
        ///     the operations.
        /// </summary>
        /// <param name="userId">The desired user.</param>
        /// <param name="collectionId">The desired Collection name.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        public bool SetUserAndCollection(string userId, string collectionId)
        {
            UserId = userId;
            CollectionId = collectionId;
            CollectionPath = Path.GetFullPath(Root) + "\\" + userId + "\\Calendars\\" + collectionId;
            return ExistCalendarCollection();
        }

        /// <summary>
        ///     Create an instance of this class and check if the collection is valid..
        /// </summary>
        /// <param name="userId">The owner of the collection.</param>
        /// <param name="collectionId">The desired collection.</param>
        /// <param name="fileSystemManagement">The instance of the class.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        public bool CreateAndCheck(string userId, string collectionId, out IFileSystemManagement fileSystemManagement)
        {
            fileSystemManagement = new FileSystemManagement(userId, collectionId);
            return fileSystemManagement.ExistCalendarCollection();
        }

        public bool AddUserFolder(string userEmail)
        {
            var path = Path.GetFullPath(Root) + "\\" + userEmail;
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }

        public bool AddCalendarCollectionFolder(string userEmail, string calendarCollectionName)
        {
            var path = Path.GetFullPath(Root) + "\\" + userEmail + "\\Calendars\\" + calendarCollectionName;
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }


        public bool DeleteCalendarCollection()
        {
            if (!Directory.Exists(CollectionPath)) return false;
            Directory.Delete(CollectionPath, true);
            return true;
        }

        public bool AddCalendarObjectResourceFile(string objectResourceName, string bodyIcalendar)
        {
          

            //Check Directory
            if (!Directory.Exists(CollectionPath))
                return false;


            //Parse the iCalendar Object
            //Construimos el objeto pa verificar que esta bien
            var iCalendar = VCalendar.Parse(bodyIcalendar);
            if (iCalendar == null)
                return false;

            //Write to Disk the toString of the object, so it splits the lines
            //
            using (StreamWriter writer = File.CreateText(CollectionPath + "\\" + objectResourceName + @".ics"))
            {
                writer.Write(iCalendar.ToString());
            }
            return true;
        }

        public string GetCalendarObjectResource(string objectResourceName)
        {
            var path = CollectionPath + "\\" + objectResourceName;
            if (File.Exists(path))
            {
                string result;
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var reader = new StreamReader(stream);
                    result= reader.ReadToEnd();
                }

                return result;
            }
            return null;
        }

        public bool DeleteCalendarObjectResource(string objectResourceName)
        {
            var path = CollectionPath + "\\" + objectResourceName;
            if (!File.Exists(path))
                return false;
            File.Delete(path);
            return true;
        }

        public bool ExistCalendarCollection()
        {
            return Directory.Exists(CollectionPath);
        }

        public bool ExistCalendarObjectResource(string objectResourceName)
        {
            var path = CollectionPath + "\\" + objectResourceName;
            return File.Exists(path);
        }

        public IEnumerable<VCalendar> GetAllCalendarObjectResource()
        {
            var calendarObjectResources = new List<VCalendar>();

            string body;
            VCalendar iCalendar;

            if (!Directory.Exists(CollectionPath))
                return null;

            var filesPath = Directory.EnumerateFiles(CollectionPath);

            foreach (var file in filesPath)
            {
                body = GetCalendarObjectResource(file);
                if (body != null)
                {
                    iCalendar = new VCalendar(body);
                    calendarObjectResources.Add(iCalendar);
                }
            }
            return calendarObjectResources;
        }

        /// <summary>
        ///     Get all the resources of a collection.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userCollectionId"></param>
        /// <param name="calendarObjectResources"></param>
        /// <returns></returns>
        public bool GetAllCalendarObjectResource(out Dictionary<string, string> calendarObjectResources)
        {
            calendarObjectResources = new Dictionary<string, string>();
            if (!Directory.Exists(CollectionPath))
                return false;
            var filesPath = Directory.EnumerateFiles(CollectionPath);
            foreach (var file in filesPath)
            {
                var temp = GetCalendarObjectResource(file);
                if (temp != null)
                    calendarObjectResources.Add(CollectionPath + "\\"+file, temp);
            }
            return true;
        }
    }
}