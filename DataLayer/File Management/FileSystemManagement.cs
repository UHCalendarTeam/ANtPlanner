using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ICalendar.Calendar;

namespace DataLayer
{
    public class FileSystemManagement : IFileSystemManagement
    {
        public bool AddPrincipalFolder(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }

        public bool AddCalendarCollectionFolder(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);

            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }


        public bool DeleteCalendarCollection(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            if (!Directory.Exists(path)) return false;
            Directory.Delete(path, true);
            return true;
        }

        public async Task<bool> AddCalendarObjectResourceFile(string url, string body)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            var colPath = path.Remove(path.LastIndexOf(Path.DirectorySeparatorChar));
            //Check Directory
            if (!Directory.Exists(colPath))
                return false;

            //Write to Disk the toString of the object, so it splits the lines
            //
            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(body);
            }

            return true;
        }

        public async Task<string> GetCalendarObjectResource(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var reader = new StreamReader(stream);
                    var result = await reader.ReadToEndAsync();
                    return result;
                }
            }
            return null;
        }

        public bool DeleteCalendarObjectResource(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            if (!File.Exists(path))
                return false;
            File.Delete(path);
            return true;
        }

        public bool ExistCalendarCollection(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            return Directory.Exists(path);
        }

        public bool ExistCalendarObjectResource(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            return File.Exists(path);
        }

        public async Task<IEnumerable<VCalendar>> GetAllCalendarObjectResource(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            var calendarObjectResources = new List<VCalendar>();

            if (!Directory.Exists(path))
                return null;

            var filesPath = Directory.EnumerateFiles(path);

            foreach (var file in filesPath)
            {
                var body = await GetCalendarObjectResource(file);
                if (body != null)
                {
                    var iCalendar = new VCalendar(body);
                    calendarObjectResources.Add(iCalendar);
                }
            }
            return calendarObjectResources;
        }

        /// <summary>
        ///     Get all the resources of a collection.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="calendarObjectResources"></param>
        /// <returns></returns>
        public async Task<bool> GetAllCalendarObjectResource(string url,
            Dictionary<string, string> calendarObjectResources)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            if (!Directory.Exists(path))
                return false;
            var filesPath = Directory.EnumerateFiles(path);

            foreach (var file in filesPath)
            {
                var lstIndex = file.LastIndexOf("\\", StringComparison.Ordinal);
                var fileName = file.Substring(lstIndex + 1);
                var temp = await GetCalendarObjectResource(file);
                if (temp != null)
                    calendarObjectResources.Add(url + fileName, temp);
            }
            return true;
        }

        public long GetFileSize(string url)
        {
            url = RemoveInitialSlash(url);
            var path = url.Replace('/', Path.DirectorySeparatorChar);
            var finfo = new FileInfo(path);
            return finfo.Length;
        }


        private string RemoveInitialSlash(string url)
        {
            if (url[0] == '/')
                return url.Substring(1);
            return url;
        }
    }
}