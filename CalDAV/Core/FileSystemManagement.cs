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
    public class FileSystemMangement : IFileSystemManagement
    {
        public string Root { get; }

        public FileSystemMangement(string root ="/CalDav/Users")
        {
            
            if (!string.IsNullOrEmpty(root) && Uri.IsWellFormedUriString(root, UriKind.Relative) && Path.IsPathRooted(root) && root !="/CalDav/Users")
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

        public bool GetAllCalendarObjectResource(string userEmail, string calendarCollectionName,out List<string> calendarObjectResources )
        {
            calendarObjectResources = new List<string>();
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            if (!Directory.Exists(path))
                return false;
            var filesPath = Directory.EnumerateFiles(path);
            foreach (var files in filesPath)
            {
                var temp = GetCalendarObjectResource(userEmail, calendarCollectionName, files);
                if(temp != null)
                    calendarObjectResources.Add(temp);
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

        public bool AddCalendarObjectResourceFile(string userEmail, string calendarCollectionName, string bodyIcalendar,
            out string objectResourceName)
        {
            var path = Path.GetFullPath(Root) + "/" + userEmail + "/Calendars/" + calendarCollectionName;
            objectResourceName = null;

            if (!Directory.Exists(path)) return false;

            TextReader reader = new StringReader(bodyIcalendar);

            var iCalendar = Parser.CalendarBuilder(reader);
            if (iCalendar == null) return false;

            var uniqueName = "";
            if (iCalendar.CalendarComponents.Count>0)
            {
                IList<IComponentProperty> list;
                if(iCalendar.Properties.TryGetValue("UID", out list ))
                {
                    var firstOrDefault = list.FirstOrDefault();
                    if (firstOrDefault != null) uniqueName = ((IValue<string>)firstOrDefault).Value.ToLower();
                }
            }
            var stream = new FileStream(path + uniqueName, FileMode.CreateNew);
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
    }
}
