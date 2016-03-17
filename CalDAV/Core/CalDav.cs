using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;

namespace CalDAV.Core
{
	// This project can output the Class library as a NuGet Package.
	// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
	public class CalDav : ICalDav
	{
		private IFileSystemManagement StorageManagement { get; }

		private IPrecondition preconditionCheck { get; set; }
		private IPostcondition postconditionCheck { get; }

		private IStartUp StartUp { get; set; }

		public CalDav(IFileSystemManagement fsManagement)
		{
			StorageManagement = fsManagement;
		}

		public string MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body)
		{
			var properties = XMLParsers.XMLMKCalendarParser(body);
			StartUp.CreateCollectionForUser(propertiesAndHeaders["userEmail"], propertiesAndHeaders["collectionName"]);
			return "";

		}
		//TODO: ADriano
		public string PropFind(Dictionary<string, string> propertiesAndHeaders, string body)
		{
            var xmlTree = XMLParsers.GenericParser(body);

            if (xmlTree.NodeName != "propfind")
                return null;

            //var propType = xmlTree.Children[0];
            //if(propType.NodeName == "prop")
            //    return  
			//REQUEST PROPERTIES
			//prop property return the value of the specified property
			//allprop property return the value of all properties --the include elemen makes the server return 
			//other properties that will not be returnes otherwise
			//getetag property return the etag of the COR for sincro options.

            
			throw new NotImplementedException();
		}

		//TODO: Nacho
		public string Report(Dictionary<string, string> propertiesAndHeaders, string body)
		{
			throw new NotImplementedException();
		}

		#region PUT resource
		//TODO: Nacho
		public bool AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag)
		{
			#region Extracting Properties
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var calendarResourceId = propertiesAndHeaders["calendarResourceID"];
			var etag = propertiesAndHeaders["etag"];
			var body = propertiesAndHeaders["body"];
			#endregion

			retEtag = null;

			//Note: calendar object resource = COR

			//CheckAllPreconditions
			preconditionCheck = new PutPrecondition(StorageManagement);
			if (!preconditionCheck.PreconditionsOK(propertiesAndHeaders))
				return false;
			
			//etag value of "If-Match"
			string updateEtag;

			if (propertiesAndHeaders.TryGetValue("If-Match", out updateEtag))
			{
				//Taking etag from If-Match header.
				propertiesAndHeaders["etag"] = updateEtag;
				return UpdateCalendarObjectResource(propertiesAndHeaders, out retEtag);
			}
			else if (propertiesAndHeaders.ContainsKey("If-Non-Match"))
			{
				return CreateCalendarObjectResource(propertiesAndHeaders, out retEtag);
			}
			else
			{
                //update or create
				using (var db = new CalDavContext())
				{
					if (db.CalendarResourceExist(userEmail, collectionName, calendarResourceId) &&
						StorageManagement.ExistCalendarObjectResource(userEmail, collectionName, calendarResourceId))
					{
						return UpdateCalendarObjectResource(propertiesAndHeaders,
							out retEtag);
					}
					return CreateCalendarObjectResource(propertiesAndHeaders, out retEtag);
				}

            }
			//return HTTP 201 Created 
		}

		/// <summary>
		/// Creates a new COR from a PUT when a "If-Non-Match" header is included
		/// </summary>
		/// <param name="propertiesAndHeaders"></param>
		/// <param name="retEtag"></param>
		/// <param></param>
		private bool CreateCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag)
		{
			#region Extracting Properties
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var calendarResourceId = propertiesAndHeaders["calendarResourceId"];
			var etag = propertiesAndHeaders["etag"];
			var body = propertiesAndHeaders["body"];
			#endregion

			TextReader reader = new StringReader(body);
			var iCal = new VCalendar(body);
			retEtag = etag;

			using (var db = new CalDavContext())
			{
				if (!db.CollectionExist(userEmail, collectionName))
					return false;

				//TODO:Calculate Etag

				//filling the resource
				var resource = FillResource(propertiesAndHeaders, db, iCal, out retEtag);
				//adding the resource to the db
				db.CalendarResources.Add(resource);

				//adding the file
				StorageManagement.AddCalendarObjectResourceFile(userEmail, collectionName, calendarResourceId, body);


				return true;
			}

		}

		/// <summary>
		/// Updates an existing COR from a PUT when a "If-Match" header is included using the corresponding etag.
		/// </summary>
		///<param name="propertiesAndHeaders"></param>
		/// <param name="retEtag"></param>
		private bool UpdateCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag)
		{
			#region Extracting Properties
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var calendarResourceID = propertiesAndHeaders["calendarResourceId"];
			var etag = propertiesAndHeaders["etag"];
			var body = propertiesAndHeaders["body"];
			#endregion

			TextReader reader = new StringReader(body);
			var iCal = new VCalendar(body);

			retEtag = etag;

			//This means that the object in the body is not correct
			if (iCal == null)
				return false;

			using (var db = new CalDavContext())
			{
				if (!db.CollectionExist(userEmail, collectionName) || !db.CalendarResourceExist(userEmail, collectionName, calendarResourceID))
					return false;

				//Fill the resource
				var resource = FillResource(propertiesAndHeaders, db, iCal, out retEtag);
				var prevResource = db.GetCalendarResource(userEmail, collectionName, calendarResourceID);
				int prevEtag;
				int actualEtag;
				string tempEtag = prevResource.Etag;
				if (int.TryParse(tempEtag, out prevEtag) && int.TryParse(retEtag, out actualEtag))
				{
					if (actualEtag > prevEtag)
					{
						if (resource.Uid != prevResource.Uid)
							return false;
						//Adding to the dataBase
						db.CalendarResources.Update(resource);

						//Removing old File 
						StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, calendarResourceID);
						//Adding New File
						StorageManagement.AddCalendarObjectResourceFile(userEmail, collectionName, calendarResourceID, body);

					}
					else
						retEtag = tempEtag;
				}
				else
					return false;

			}
			return true;
		}

		/// <summary>
		/// Method in charge of fill a CalendarResource and Return it.
		/// </summary>
		/// <param name="propertiesAndHeaders"></param>
		/// <param name="db"></param>
		/// <param name="iCal"></param>
		/// <param name="retEtag"></param>
		/// <returns></returns>
		private CalendarResource FillResource(Dictionary<string, string> propertiesAndHeaders, CalDavContext db, VCalendar iCal, out string retEtag)
		{
			#region Extracting Properties
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var calendarResourceID = propertiesAndHeaders["calendarResourceId"];
			var etag = propertiesAndHeaders["etag"];
			#endregion
		  //TODO: aki estabas cogiendo las propiedades del VCALENDAR
			//para coger las del CalComponent tienes que buscarlo
			//en iCal.CalendarComponents y coger la q no es VTIMEZONE

			CalendarResource resource = new CalendarResource();
			
			resource.User = db.GetUser(userEmail);
			resource.Collection = db.GetCollection(userEmail, collectionName);
			IComponentProperty property;
			property = iCal.GetComponentProperties("DTSTART");
			if (property != null)
			{
				resource.DtStart = ((IValue<DateTime>) property).Value;
			}
			property = iCal.GetComponentProperties("DTEND");
			if (property != null)
			{
				resource.DtEnd = ((IValue<DateTime>)property).Value;
			}
			property = iCal.GetComponentProperties("UID");
			if (property!=null)
			{
				resource.Uid = ((IValue<string>)property).Value;
			}
			//TODO: Take the resource Etag if the client send it if not assign one
			if (etag != null)
				resource.Etag = etag;
			else
			{
				//resource.Etag    
			}
			retEtag = resource.Etag;

			resource.FileName = calendarResourceID;

			resource.UserId = resource.User.UserId;

			resource.ResourceType = iCal.CalendarComponents.Keys.Where(k => k != "VTIMEZONE").Single();

			//TODO: Recurrence figure out how to assign this
			//resource.Recurrence =

			return resource;
		}
		#endregion

		//TODO:Nacho
		public string PropPatch(Dictionary<string, string> propertiesAndHeaders, string Body)
		{
			throw new NotImplementedException();
		}

		public bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders)
		{
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var calendarResourceId = propertiesAndHeaders["calendarResourceId"];
			using (var db = new CalDavContext())
			{
				var resource = db.GetCollection(userEmail, collectionName).CalendarResources.First(x => x.FileName == calendarResourceId);
				db.CalendarResources.Remove(resource);
				db.SaveChanges();
			}
			return StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, calendarResourceId);
		}

		public bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
		{
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var resourceId = propertiesAndHeaders["resourceId"];
			using (var db = new CalDavContext())
			{
				var collection = db.GetCollection(userEmail, collectionName);
				if (collection == null)
					return false;
				db.CalendarCollections.Remove(collection);
				return StorageManagement.DeleteCalendarCollection(userEmail, collectionName);


			}


		}

		public string ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string etag)
		{
			var userEmail = propertiesAndHeaders["userEmail"];
			var collectionName = propertiesAndHeaders["collectionName"];
			var calendarResourceId = propertiesAndHeaders["calendarResourceId"];

			//Must return the Etag header of the COR

			using (var db = new CalDavContext())
			{
				var calendarRes = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
				etag = calendarRes.Etag;
			}
			return StorageManagement.GetCalendarObjectResource(userEmail, collectionName, calendarResourceId);
		}

		public string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
		{
			throw new NotImplementedException();
		}

	}
}
