using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using Microsoft.SqlServer.Server;
using TreeForXml;

namespace CalDAV.Models.Method_Extensions
{
    /// <summary>
    /// This class contains method extensions for 
    /// IEnumerable<VCALENDAR>
    /// </summary>
    public static class ExtensionsForFilters
    {
        /// <summary>
        /// Apply the filter to the given calendar.
        /// </summary>
        /// <param name="calendar">THe calendar to apply the filter.</param>
        /// <param name="filterTree">The filter container.</param>
        /// <returns>True fi the calendar pass the filter, false otherwise</returns>
        public static bool FilterResource(this VCalendar calendar, IXMLTreeStructure filterTree)
        {
            //first get the component in the calendar where to apply the filter.
            IXMLTreeStructure filtersContainer = null;
            ICalendarComponent component = null;
            if(!RecursiveSeeker(calendar, filterTree,out filtersContainer,out component))
                return false;//if the container doesnt have the requested comp ther return false;
            bool result = true;
            foreach (var filter in filtersContainer.Children)
            {
                switch (filter.NodeName)
                {
                    case "prop-filter":
                        result = component.PropertyFilter(filter);
                        if (!result)
                            return false;
                        break;
                    
                    default:
                        throw new NotImplementedException($"The filter {filter.NodeName} isn't implemented");
                    
                }
            }
            return true;
        }

        /// <summary>
        /// Apply filters to a property
        /// </summary>
        /// <param name="component">The component where to apply the filters.</param>
        /// <param name="filter">Filters container.</param>
        /// <returns>True if the component pass the filters, false otherwise.</returns>
        private static bool PropertyFilter(this ICalendarComponent component, IXMLTreeStructure filter)
        {
            var propName = filter.Attributes["name"];
            //if the comp doesnt has the desired prop return false
           
            IComponentProperty propValue;
            foreach (var propFilter in filter.Children)
            {
                bool result;
                switch (propFilter.NodeName)
                {
                    case "text-match":
                        if (!component.Properties.TryGetValue(propName, out propValue))
                            return false;
                        result = propValue.StringValue.TextMatchFilter(propFilter);
                        if (!result)
                            return false;
                        break;
                    case "param-filter":
                        if (!component.Properties.TryGetValue(propName, out propValue))
                            return false;
                        result = propValue.ParamFilter(propFilter);
                        if (!result)
                            return false;
                        break;
                    case "is-not-defined":
                        if (component.Properties.TryGetValue(propName, out propValue))
                            return false;
                        break;
                    default:
                        throw new NotImplementedException($"THe property filter {propFilter.NodeName} is not implemented");
                }
            }
            return true;
        }

        private static bool ParamFilter(this IComponentProperty property, IXMLTreeStructure filters)
        {
            var paramName = filters.Attributes["name"];
            var param = property.PropertyParameters.First(x => x.Name == paramName);
            if (param == null)
                return false;
            foreach (var filter in filters.Children)
            {
                switch (filter.NodeName)
                {
                    case "text-match":
                        var result = param.Value.TextMatchFilter(filter);
                        if (!result)
                            return false;
                        break;
                    default:
                        throw new NotImplementedException("Implement the filter name or return an error");
                }
            }
            return true;
        }

        private static bool TextMatchFilter(this string propertyValue, IXMLTreeStructure filter)
        {
            //TODO: coger aki el value del property de tipo string cuando este implementado.
            bool negateCond = false;
            string negCondStr;
            //if the filter contains a negate condition attr then take it
            if (filter.Attributes.TryGetValue("negate-condition", out negCondStr))
                negateCond = negCondStr == "yes";
            bool result;
            switch (filter.Attributes["collation"])
            {
                case "i;octet":
                    byte[] propValueOctet = System.Text.Encoding.ASCII.GetBytes(propertyValue);
                    byte[] filterValueOctet = System.Text.Encoding.ASCII.GetBytes(filter.Value);
                    result= ApplyTextFilter(propValueOctet, filterValueOctet);
                    return negateCond ? !result : result;
                    
                case "i;ascii-casemap":
                    var propValueAscii = propertyValue.Select(x => (int) x).ToArray();
                    var filterValueAscii = propertyValue.Select(x => (int) x).ToArray();
                    result= ApplyTextFilter(propValueAscii, filterValueAscii);
                    return negateCond ? !result : result;
                default:
                    throw new NotImplementedException("Implement the error for return");
            }
            
        }


        public static bool ApplyTextFilter<T>(T[] propValue, T[] filterValue)
        {
            // The substring operation returns "match" if the first string is the
           // empty string, or if there exists a substring of the second string of
            //length equal to the length of the first string, which would result in
            // a "match" result from the equality function.Otherwise, the
            //substring operation returns "no-match".
            if (filterValue.Length == 0)
                return true;
            if (filterValue.Length > propValue.Length)
                return false;
            for (int i = 0; i < filterValue.Length; i++)
            {
                if (!propValue[i].Equals(filterValue[i]))
                    return false;
            }
            return true;
        }


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
        /// <summary>
        /// Go deeper in the calCOmponents following the treeStructure
        /// till it get the component where to apply the filter.
        /// </summary>
        /// <param name="container">The container of the components.</param>
        /// <param name="treeStructure">The IXMLTree where is the filter.</param>
        /// <param name="filter">Return The node that contains the filter.</param>
        /// <param name="component">THe component where to apply the filter.</param>
        /// <returns>True if found the component, false otherwise.</returns>
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
