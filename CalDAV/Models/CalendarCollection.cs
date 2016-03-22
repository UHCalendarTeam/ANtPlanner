using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using CalDAV.CALDAV_Properties.Interfaces;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.Models
{
    /// <summary>
    /// To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection: IDavProperties
    {
        public int CalendarCollectionId { get; set; }

        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string DisplayName { get; set; }
        public string CalendarDescription { get; set; }

        public string CalendarTimeZone { get; set; }
       /* [Optional]
        public ICollection<string> SupportedCalendarComponentSet { get; set; }*/

            
        //public List<string> ResourceType { get; set; } 

        public int MaxResourceSize { get; set; }

        public DateTime MinDateTime { get; set; }

        public DateTime MaxDateTime { get; set; }

        public int MaxIntences { get; set; }

        public ICollection<CalendarResource> CalendarResources { get; set; }

        public DateTime CreationDate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int GetContentLenght
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string GetContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string GetEtag
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime GetLastModified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string GetContentLanguage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public XmlTreeStructure LockDiscovery
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public XmlTreeStructure ResourceType { get; set; }

        public XmlTreeStructure SupportedLock
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        
    }
}