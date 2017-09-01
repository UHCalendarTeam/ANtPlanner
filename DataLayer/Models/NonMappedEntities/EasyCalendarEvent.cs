using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.NonMappedEntities
{
    public class EasyCalendarEvent
    {

        public string title { get; set; }

        public string description { get; set; }

        public string start { get; set; }

        public string end { get; set; }

        public string allday { get; set; }

        public string url { get; set; }

        public string color { get; set; }

        public string borderColor { get; set; }

        public string textColor { get; set; }


        public EasyCalendarEvent()
        {
        }

        public EasyCalendarEvent(string title, string description, string start, string end, string allday, string url, string color = null, string borderColor = null, string textColor = null)
        {
            this.title = title;
            this.description = description;
            this.start = start;
            this.end = end;
            this.allday = allday;
            this.url = url;
            this.color = color;
            this.borderColor = borderColor;
            this.textColor = textColor;
        }
    }
}
