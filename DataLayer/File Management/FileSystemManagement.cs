using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ICalendar.Calendar;

namespace DataLayer
{
    public class FileSystemManagement : IFileSystemManagement
    {
        /// <summary>
        ///     Use this constructor to set the root path of the files.
        /// </summary>
        /// <param name="root"></param>
        //public FileSystemManagement(string root = "CalDav\\Users")
        //{
        //    if (root != "CalDav\\Users" && !string.IsNullOrEmpty(root) &&
        //        Uri.IsWellFormedUriString(root, UriKind.Relative) && Path.IsPathRooted(root))
        //        Root = root;
        //    else
        //        Root = Directory.GetCurrentDirectory() + "\\" + root;
        //}

        //public FileSystemManagement(string userId, string collectionId, string root = "CalDav\\Users")
        //{
        //    Root = root;
        //    UserId = userId;
        //    CollectionId = collectionId;
        //    CollectionPath = Path.GetFullPath(Root) + "\\" + userId + "\\Calendars\\" + collectionId;
        //}

        /// <summary>
        ///     Contains the userId where to apply the operations.
        /// </summary>
        //public string UserId { get; set; }

        ///// <summary>
        /////     Contains the collection name where to apply the operations.
        ///// </summary>
        //public string CollectionId { get; set; }

        ///// <summary>
        /////     Contains the path to the collection where to apply the operations..
        ///// </summary>
        //public string CollectionPath { get; set; }

        ///// <summary>
        /////     Contains the root where are the collections.
        ///// </summary>
        //public string Root { get; }

        /// <summary>
        ///     After created the class with the default constructor
        ///     this method set the user and collection where to apply
        ///     the operations.
        /// </summary>
        /// <param name="userId">The desired user.</param>
        /// <param name="collectionId">The desired Collection name.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        //public static bool SetUserAndCollection(string userId, string collectionId)
        //{
        //    UserId = userId;
        //    CollectionId = collectionId;
        //    CollectionPath = Path.GetFullPath(Root) + "\\" + userId + "\\Calendars\\" + collectionId;
        //    return ExistCalendarCollection();
        //}

        /// <summary>
        ///     Create an instance of this class and check if the collection is valid..
        /// </summary>
        /// <param name="userId">The owner of the collection.</param>
        /// <param name="collectionId">The desired collection.</param>
        /// <param name="fileSystemManagement">The instance of the class.</param>
        /// <returns>True if the collection exist, false otherwise</returns>
        //public bool CreateAndCheck(string userId, string collectionId, out IFileSystemManagement fileSystemManagement)
        //{
        //   // fileSystemManagement = new FileSystemManagement(userId, collectionId);
        //    return fileSystemManagement.ExistCalendarCollection();
        //}

        public bool AddPrincipalFolder(string principalUrl)
        {
            var path = principalUrl.Replace('/', Path.DirectorySeparatorChar);
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }

        public bool AddCalendarCollectionFolder(string collectionUrl)
        {
            var path = collectionUrl.Replace('/', Path.DirectorySeparatorChar);
            var dirInfo = Directory.CreateDirectory(path);
            return dirInfo.Exists;
        }


        public bool DeleteCalendarCollection(string collectionUrl)
        {
            var path = collectionUrl.Replace('/', Path.DirectorySeparatorChar);
            if (!Directory.Exists(path)) return false;
            Directory.Delete(path, true);
            return true;
        }

        public async Task<bool> AddCalendarObjectResourceFile(string resourceUrl, string body)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            var colPath = path.Remove(path.LastIndexOf(Path.DirectorySeparatorChar));
            //Check Directory
            if (!Directory.Exists(colPath))
                return false;


            //Parse the iCalendar Object
            //Construimos el objeto pa verificar que esta bien
            var iCalendar = VCalendar.Parse(body);
            if (iCalendar == null)
                return false;

            //Write to Disk the toString of the object, so it splits the lines
            //
            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(iCalendar.ToString());
            }

            return true;
        }

        public async Task<string> GetCalendarObjectResource(string resourceUrl)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
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

        public bool DeleteCalendarObjectResource(string resourceUrl)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            if (!File.Exists(path))
                return false;
            File.Delete(path);
            return true;
        }

        public bool ExistCalendarCollection(string collectioUrl)
        {
            var path = collectioUrl.Replace('/', Path.DirectorySeparatorChar);
            return Directory.Exists(path);
        }

        public bool ExistCalendarObjectResource(string resourceUrl)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            return File.Exists(path);
        }

        public IEnumerable<VCalendar> GetAllCalendarObjectResource(string collectionUrl)
        {
            var path = collectionUrl.Replace('/', Path.DirectorySeparatorChar);
            var calendarObjectResources = new List<VCalendar>();

            string body;
            VCalendar iCalendar;

            if (!Directory.Exists(path))
                return null;

            var filesPath = Directory.EnumerateFiles(path);

            foreach (var file in filesPath)
            {
                body = GetCalendarObjectResource(file).Result;
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
        /// <param name="collectionUrl"></param>
        /// <param name="calendarObjectResources"></param>
        /// <param name="userId"></param>
        /// <param name="userCollectionId"></param>
        /// <returns></returns>
        public bool GetAllCalendarObjectResource(string collectionUrl, out Dictionary<string, string> calendarObjectResources)
        {
            var path = collectionUrl.Replace('/', Path.DirectorySeparatorChar);
            calendarObjectResources = new Dictionary<string, string>();
            if (!Directory.Exists(path))
                return false;
            var filesPath = Directory.EnumerateFiles(path);
            
            foreach (var file in filesPath)
            {
                var lstIndex = file.LastIndexOf("\\");
                var fileName = file.Substring(lstIndex + 1);
                var temp = GetCalendarObjectResource(file);
                if (temp != null)
                    calendarObjectResources.Add(collectionUrl + fileName, temp.Result);
            }
            return true;
        }

        public long GetFileSize(string resourceUrl)
        {
            var path = resourceUrl.Replace('/', Path.DirectorySeparatorChar);
            var finfo = new FileInfo(path);
            return finfo.Length;
        }
    }
}