//using System.Collections.Generic;
//using DataLayer.Models.Entities;

//namespace DataLayer
//{
//    /// <summary>
//    ///     Contains the logic for the creation of new clients of the system.
//    /// </summary>
//    public class StartUp : IStartUp
//    {
//        public StartUp(IFileManagement fileSystem)
//        {
//            FileSystemMangement = fileSystem;
//        }

//        public IFileManagement FileSystemMangement { get; set; }

//        /// <summary>
//        ///     Create a new user in the system( add him to the DB and create a directory for his collections)
//        /// </summary>
//        /// <param name="userEmail">THe email of the new user.</param>
//        /// <param name="userName">The firstName of the new user.</param>
//        /// <param name="userLastName">The lastName of the new user.</param>
//        /// <returns>True if success, false otherwise.</returns>
//        public bool CreateUserInSystem(string userEmail, string userName, string userLastName)
//        {
//            //TODO: check for the result of the user creation in the DB
//            using (var db = new CalDavContext())
//            {
//                db.Users.Add(new User {Email = userEmail, FirstName = userName, LastName = userLastName});
//                db.SaveChanges();
//                var result = FileSystemMangement.AddUserFolder(userEmail);
//            }
//            return true;
//        }

//        /// <summary>
//        ///     Create a new collection for a given user.( Add to the DB and relate it with the user,
//        ///     Create a new folder for the collection
//        /// </summary>
//        /// <param name="userEmail">The email of the collection's user.</param>
//        /// <param name="collectionName">THe name for the new collection.</param>
//        /// <param name="calendarDescription">THe calendar description.</param>
//        /// <returns>True if success, false otherwise</returns>
//        public bool CreateCollectionForUser(string userEmail, string collectionName, string calendarDescription = "",
//            string calDisplayName = "", string calTimeZone = "", List<string> supportedCalendarComponentSet = null)
//        {
//            //TODO: check for the result of the collection creation in the DB
//            using (var db = new CalDavContext())
//            {
//                var principal = db.GetPrincipal(userEmail);
//                var collection = new CalendarCollection
//                {
//                    Principal = principal,
//                    Name = collectionName,
//                    Properties = new List<Property>
//                    {
//                        new Property
//                        {
//                            Name = "calendar-description",
//                            Namespace = "C:",
//                            Value =
//                                calendarDescription == ""
//                                    ? "This is a desfault calendar collection. Should provide the calendar description"
//                                    : calendarDescription,
//                            IsVisible = true,
//                            IsMutable = true,
//                            IsDestroyable = false
//                        }
//                    }
//                    //Calendardescription = calendarDescription==""?"This is a desfault calendar collection. Should provide the calendar description":calendarDescription,
//                    //Displayname = calDisplayName

//                    //TODO: take the other properties from the class that is gonna contain the 
//                    //custom properties of the collections
//                };
//                principal.CalendarCollections.Add(collection);
//                db.SaveChanges();
//                //var result = FileSystemMangement.CreateFolder(TODO);
//                return result;
//            }
//        }
//    }
//}

