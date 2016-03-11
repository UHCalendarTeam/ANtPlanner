using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICalendar;
using ICalendar.Calendar;
using ICalendar.Utils;

namespace CalDAV.Core
{
    public class FileSystemMangement : IFileSystemManagement
    {
        public Uri Root { get; }

        public FileSystemMangement(string root = "/CalDav/Users")
        {
            if (!string.IsNullOrEmpty(root) && Uri.IsWellFormedUriString(root, UriKind.RelativeOrAbsolute))
                Root = new Uri(root);
            else
                Root = new Uri("/CalDav/Users", UriKind.Relative);
        }
        public bool AddUserFolder(string userName)
        {
            var path = Root.AbsolutePath + "/" + userName;
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }

        public bool AddCalendarFolder(string userName, string calendarCollectionName)
        {
            var path = Root.AbsolutePath + "/" + userName + "/Calendars/" + calendarCollectionName;
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }

        public bool AddCalendarObjectResourceFile(string userName, string calendarCollectionName, string bodyIcalendar,
            out string objectResourceName)
        {
            var path = Root.AbsolutePath + "/" + userName + "/Calendars/" + calendarCollectionName;
            objectResourceName = null;
            if (Directory.Exists(path))
            {
                TextReader reader = new StringReader(bodyIcalendar);
                var iCalendar = Parser.CalendarBuilder(reader);
                if (iCalendar == null) return false;
                var uniqueName = "";
                if (iCalendar.CalendarComponents.Count>0)
                     uniqueName = iCalendar.CalendarComponents.Keys.FirstOrDefault().ToLower() + DateTime.Now;
                var stream = new FileStream(path + uniqueName, FileMode.CreateNew);
                using (stream)
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(bodyIcalendar);

                }
            }
            return true;
        }

        public string GetCalendarObjectResource(string userName, string calendarCollectionName, string objectResourceName)
        {
            var path = Root.AbsolutePath + "/" + userName + "/Calendars/" + calendarCollectionName + "/" + objectResourceName;
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
    }
}
