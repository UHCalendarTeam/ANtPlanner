using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using TreeForXml;

namespace CalDAV.Models.Method_Extensions
{
    /// <summary>
    /// This class contains method extensions for 
    /// IEnumerable<VCALENDAR>
    /// </summary>
    public static class Extensions
    {

        public static Dictionary<string, VCalendar> TimeFilter(this Dictionary<string, VCalendar> resources,
            DateTime start, DateTime end, IXMLTreeStructure filter)
        {
            var output = new Dictionary<string, VCalendar>();

            foreach (var resource in resources)
            {
                ICalendarComponent comp = null;
                IXMLTreeStructure compNode = null;
                compNode = filter.GetChild("comp-filter");
                string compName = compNode.Attributes["name"];
                if (resource.Value.CalendarComponents.ContainsKey(compName))
                {
                    comp = resource.Value.CalendarComponents[compName].First();
                }
                else
                    continue;
            }
            return null;

        }

        public static bool RecursiveSeeker(ICalendarComponentsContainer container, 
            IXMLTreeStructure treeStructure, out IXMLTreeStructure filter,out ICalendarComponent component)
        {
            filter = null;
            component = null;
            while (true)
            {
                ICalendarComponent comp = null;
                IXMLTreeStructure compNode = null;
                compNode = treeStructure.GetChild("comp-filter");
                if (compNode == null)
                    //if the filter doesnt has a child with comp-filter name
                    //and the name of the container is 
                    if (((ICalendarObject) container).Name == treeStructure.Attributes["name"])
                    {
                        filter = treeStructure;
                        component = container as ICalendarComponent;
                        return true;
                    }
                    else
                        return false;
                string compName = compNode.Attributes["name"];
                //if the container doesn't has a calComponent with the desired compName
                //then return false
                if (!container.CalendarComponents.ContainsKey(compName))
                    return false;
                //take the comp with the desired name
                comp = container.CalendarComponents[compName].First();
                var componentsContainer = comp as ICalendarComponentsContainer;
                //if the the filter has more components and the container has more calendarComp
                //then go deeper
                if (componentsContainer != null && compNode.Children.Any(x => x.NodeName == "comp-filter"))
                {
                    container = componentsContainer;
                    treeStructure = compNode;
                    continue;
                }
                //if not then apply the filter in the comp
                component = comp;
                filter = compNode;
                return true;

                
                
            }
        }
    }
}
