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
        public string Root { get; }

        public FileSystemManagement(string root = "/CalDav/Users")
        {

            if (!string.IsNullOrEmpty(root) && Uri.IsWellFormedUriString(root, UriKind.Relative) && Path.IsPathRooted(root) && root != "/CalDav/Users")
                Root = root;
            else
                Root = Directory.GetCurrentDirectory() + root;
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
        //TODO: esto tiene que devolver un Dict<string, string> que tenga el path del resource y el contenido
        //
        public bool GetAllCalendarObjectResource(string userEmail, string calendarCollectionName, out Dictionary<string,string> calendarObjectResources)
        {
            calendarObjectResources = new Dictionary<string, string>();
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            if (!Directory.Exists(path))
                return false;
            var filesPath = Directory.EnumerateFiles(path);
            foreach (var file in filesPath)
            {
                var temp = GetCalendarObjectResource(userEmail, calendarCollectionName, file);
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
