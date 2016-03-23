using System;
using System.ComponentModel.DataAnnotations;
using CalDAV.CALDAV_Properties.Interfaces;
using CalDAV.Core;
using CalDAV.Utils.XML_Processors;
using TreeForXml;

namespace CalDAV.Models
{
    /// <summary>
    /// to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource: IDavProperties
    {
        [ScaffoldColumn(false)]
        public int CalendarResourceId { get; set; }

        [Required]
        public string FileName { get; set; }

        public string GetEtag { get; set; }

        public DateTime DtStart { get; set; }

        /// <summary>
        /// The endDateTime of the resource if defined.
        /// Default value = DateTime.Max
        /// </summary>
        public DateTime DtEnd { get; set; }

        public string Recurrence { get; set; }

        public string Uid { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// The owner of the resource
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The collection where the resource is defined.
        /// </summary>
        public CalendarCollection Collection { get; set; }

        ///// <summary>
        ///// Define the comp type(i.e. VEVENT, VTODO)
        ///// </summary>
        //public string ResourceType { get; set; }

        /// <summary>
        /// The duration of the resource if defined
        /// Default value ="".
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Returns the datetime value of when was created the resource.
        /// </summary>
        //public string CreationDate { get; set; }
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Returns the Lenght of the file.
        /// </summary>
        public string GetContentLength { get; }

        public string DisplayName
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

        public XmlTreeStructure ResourceType => null;

        public XmlTreeStructure SupportedLock
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}