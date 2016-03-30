using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICalendar;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using ICalendar.Utils;

namespace CalDAV.Core
{
    public class FileSystemManagement : IFileSystemManagement
    {
        /// <summary>
        /// Contains the root where are the collections.
        /// </summary>
        public string Root { get; }

        /// <summary>
        /// Contains the userId where to apply the operations.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Contains the collection name where to apply the operations.
        /// </summary>
        public string CollectionId { get; set; }

        /// <summary>
        /// Use this construtor to set the root path of the files.
        /// </summary>
        /// <param name="root"></param>
        public FileSystemManagement(string root = "/CalDav/Users")
        {

            if (root != "/CalDav/Users" && !string.IsNullOrEmpty(root) && Uri.IsWellFormedUriString(root, UriKind.Relative) && Path.IsPathRooted(root))
                Root = root;
            else
                Root = Directory.GetCurrentDirectory() + root;
        }

        public FileSystemManagement(string userId, string collectionId)
        {
            UserId = userId;
            CollectionId = collectionId;
        }

        public bool AddUserFolder(string userEmail)
        {
            var path = Path.GetFullPath(Root) + "\\" + userEmail;
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }

        public bool AddCalendarCollectionFolder(string userEmail, string calendarCollectionName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }
       
        /// <summary>
        /// Get all the resources of a collection.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userCollectionId"></param>
        /// <param name="calendarObjectResources"></param>
        /// <returns></returns>
        public bool GetAllCalendarObjectResource(out Dictionary<string,string> calendarObjectResources)
        {
            calendarObjectResources = new Dictionary<string, string>();
            var path = Path.GetFullPath(Root) + "/" + userId + "/Calendars/" + userCollectionId;
            if (!Directory.Exists(Path))
                return false;
            var filesPath = Directory.EnumerateFiles(path);
            foreach (var file in filesPath)
            {
                var temp = GetCalendarObjectResource(userId, userCollectionId, file);
                if (temp != null)
                    calendarObjectResources.Add(path+"/file", temp);
            }
            return true;
        }

        public bool DeleteCalendarCollection(string userEmail, string calendarCollectionName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            if (!Directory.Exists(path)) return false;
            Directory.Delete(path, true);
            return true;
        }

        public bool AddCalendarObjectResourceFile(string userEmail, string calendarCollectionName, string objectResourceName, string bodyIcalendar)
        {

            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            objectResourceName = null;

            //Check Directory
            if (!Directory.Exists(path)) return false;



            //Parse the iCalendar Object
            //TODO: pa q estas construyendo esto??
            var iCalendar = new VCalendar(bodyIcalendar);
            if (iCalendar == null) return false;

            //Write to Disk
            var stream = new FileStream(path + objectResourceName, FileMode.CreateNew);
            using (stream)
            {
                var writer = new StreamWriter(stream);
                writer.Write(bodyIcalendar);

            }
            return true;
        }

        public string GetCalendarObjectResource(string userEmail, string calendarCollectionName, string objectResourceName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName + "/" + objectResourceName;
            if (File.Exists(path))
            {
                var stream = new FileStream(path, FileMode.Open);
                using (stream)
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }

            }
            return null;
        }

        public bool DeleteCalendarObjectResource(string userEmail, string calendarCollectionName, string objectResourceName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName + "/" + objectResourceName;
            if (!File.Exists(path))
                return false;
            File.Delete(path);
            return true;

        }

        public bool ExistCalendarCollection(string userEmail, string calendarCollectionName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            return Directory.Exists(path);
        }

        public bool ExistCalendarObjectResource(string userEmail, string calendarCollectionName, string objectResourceName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName + "/" + objectResourceName;
            return File.Exists(path);
        }

        public IEnumerable<VCalendar> GetAllCalendarObjectResource(string userEmail, string calendarCollectionName)
        {
            var calendarObjectResources = new List<VCalendar>();
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            
            string body;
            VCalendar iCalendar;

            if (!Directory.Exists(path))
                return null;

            var filesPath = Directory.EnumerateFiles(path);

            foreach (var file in filesPath)
            {
                body = GetCalendarObjectResource(userEmail, calendarCollectionName, file);
                if (body != null)
                {
                    iCalendar = new VCalendar(body);
                    calendarObjectResources.Add(iCalendar);
                }

            }
            return calendarObjectResources;
        }
      
    }
}
