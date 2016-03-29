using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICalendar.Calendar;
using ICalendar.CalendarComponents;
using ICalendar.ComponentProperties;
using ICalendar.GeneralInterfaces;
using ICalendar.Utils;
using ICalendar.ValueTypes;
using Remotion.Linq.Clauses;
using TreeForXml;

namespace CalDAV.Core.Method_Extensions
{
    /// <summary>
    ///     This class contains method extensions for
    ///     IEnumerable<VCALENDAR>
    /// </summary>
    public static class ExtensionsForFilters
    {
        /// <summary>
        ///     Apply different filters to the given calendar.
        /// </summary>
        /// <param name="calendar">THe calendar to apply the filter.</param>
        /// <param name="filterTree">The filter container.</param>
        /// <returns>True if the calendar pass the filter, false otherwise</returns>
        public static bool FilterResource(this VCalendar calendar, IXMLTreeStructure filterTree)
        {
            //first get the component in the calendar where to apply the filter.
            IXMLTreeStructure filtersContainer = null;
            ICalendarComponent component = null;
            if (!ComponentSeeker(calendar, filterTree, out filtersContainer, out component))
                return false; //if the container doesnt have the requested comp ther return false;
            var result = true;
            foreach (var filter in filtersContainer.Children)
            {
                switch (filter.NodeName)
                {
                    case "prop-filter":
                        result = component.PropertyFilter(filter);
                        if (!result)
                            return false;
                        break;

                    case "time-range":
                        result = component.ApplyTimeFilter(filter);
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
        ///     Apply the given filters to to a property in the cal component.
        /// </summary>
        /// <param name="component">The component where to apply the filters.</param>
        /// <param name="filter">Filters container.</param>
        /// <returns>True if the component pass the filters, false otherwise.</returns>
        private static bool PropertyFilter(this ICalendarComponent component, IXMLTreeStructure filter)
        {
            //take the property where to apply the filter.
            var propName = filter.Attributes["name"];
            //if the comp doesn't has the desired prop return false

            ///iterate over the filters, if one is false then
            ///returns false
            IComponentProperty propValue;

            ///this is gonna be use for the ATTENDEE and RRULE 
            /// properties
            List<IComponentProperty> propMultiValues;
            foreach (var propFilter in filter.Children)
            {
                bool result=false;
                switch (propFilter.NodeName)
                {
                    case "text-match":
                        ///have to check this line in each of the cases because the
                        ///"is not defined"
                        if (component.Properties.TryGetValue(propName, out propValue))
                            result = propValue.StringValue.TextMatchFilter(propFilter);
                        else if (component.MultipleValuesProperties.TryGetValue(propName, out propMultiValues))
                            result = propMultiValues.Any(x => x.StringValue.TextMatchFilter(propFilter));


                        if (!result)
                            return false;
                        break;

                    case "param-filter":
                        if (component.Properties.TryGetValue(propName, out propValue))
                            result = propValue.ParamFilter(propFilter);
                        else if (component.MultipleValuesProperties.TryGetValue(propName, out propMultiValues))
                            result = propMultiValues.Any(x => x.ParamFilter(propFilter));
                        if (!result)
                            return false;
                        break;

                    case "is-not-defined":
                        ///if the component has a single prop with the given na,e
                        /// return false
                        if (component.Properties.ContainsKey(propName))
                            return false;
                        ///else if contains a multiple property with the given name
                        /// returns false
                        if (component.GetMultipleCompProperties(propName).Any())
                            return false;
                        break;

                    default:
                        throw new NotImplementedException(
                            $"THe property filter {propFilter.NodeName} is not implemented");
                }
            }
            return true;
        }

        /// <summary>
        ///     Apply a filter to the param of the property with name
        ///     equal the given param
        /// </summary>
        /// <param name="component">The component where to apply the filters.</param>
        /// <param name="filter">Filters container.</param>
        /// <returns>True if the param pass the filters, false otherwise.</returns>
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
                        throw new NotImplementedException(@"The filter {filter.NodeName} is not implemented yet.");
                }
            }
            return true;
        }

        /// <summary>
        ///     Apply a text-match filter to the given value.
        /// </summary>
        /// <param name="propertyValue">The property value where to apply the filter.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>True if pass the filter, false otherwise.</returns>
        private static bool TextMatchFilter(this string propertyValue, IXMLTreeStructure filter)
        {
            var negateCond = false;
            string negCondStr;
            //if the filter contains a negate condition attr then take it
            if (filter.Attributes.TryGetValue("negate-condition", out negCondStr))
                negateCond = negCondStr == "yes";
            bool result;
            //add the default collation if the node doesnt contains one.
            if (!filter.Attributes.ContainsKey("collation"))
                filter.Attributes["collation"] = "i;ascii-casemap";
            switch (filter.Attributes["collation"])
            {
                case "i;octet":
                    var propValueOctet = Encoding.ASCII.GetBytes(propertyValue);
                    var filterValueOctet = Encoding.ASCII.GetBytes(filter.Value);
                    result = ApplyTextFilter(propValueOctet, filterValueOctet);
                    return negateCond ? !result : result;

                case "i;ascii-casemap":
                    var propValueAscii = propertyValue.Select(x => (int) x).ToArray();
                    var filterValueAscii = filter.Value.Select(x => (int) x).ToArray();
                    result = ApplyTextFilter(propValueAscii, filterValueAscii);
                    return negateCond ? !result : result;

                default:
                    throw new NotImplementedException("Implement the error for return");
            }
        }

        /// <summary>
        ///     Compares to values and see if the filterValue is
        ///     is container in the propValue.
        /// </summary>
        /// <param name="propValue">The property value .</param>
        /// <param name="filterValue">Filters container.</param>
        /// <returns>True if the component pass the filters, false otherwise.</returns>
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
            for (var i = 0; i < filterValue.Length; i++)
            {
                if (!propValue[i].Equals(filterValue[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Apply the time-filter to the 
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Dictionary<string, VCalendar> TimeFilter(this Dictionary<string, VCalendar> resources,
            DateTime start, DateTime end, IXMLTreeStructure filter)
        {
            var output = new Dictionary<string, VCalendar>();

            foreach (var resource in resources)
            {
                ICalendarComponent comp = null;
                IXMLTreeStructure compNode = null;
                compNode = filter.GetChild("comp-filter");
                var compName = compNode.Attributes["name"];
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
        ///     The filters have to be applied in an specific component, property
        ///     or param of a property. This method go deep in the calCOmponents following the
        ///     treeStructure of the filter till it get the component where to apply the filter.
        /// </summary>
        /// <param name="container">The container of the components.</param>
        /// <param name="treeStructure">The IXMLTree where is the filter.</param>
        /// <param name="filter">Return The XmlTreeStructure node that contains the filter.</param>
        /// <param name="component">The final component where to apply the filter.</param>
        /// <returns>True if found the component, false otherwise.</returns>
        public static bool ComponentSeeker(this ICalendarComponentsContainer container,
            IXMLTreeStructure treeStructure, out IXMLTreeStructure filter, out ICalendarComponent component)
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
                var compName = compNode.Attributes["name"];
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

        /// <summary>
        ///     Apply the time filter to the given component as
        ///     defined in 9.9 CALDAV:time-range XML Element.
        /// </summary>
        /// <param name="component">The component where to apply the filter.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>True if the component pass the filter, false otherwise.</returns>
        private static bool ApplyTimeFilter(this ICalendarComponent component, IXMLTreeStructure filter)
        {
            DateTime? start;
            DateTime? end;

            ///get the start and time attributes of the filter.
            if(filter.Attributes.ContainsKey("start"))
                filter.Attributes["start"].ToDateTime(out start);
            else //if not then assign infinite
                start = DateTime.MinValue;

            if (filter.Attributes.ContainsKey("end"))
                filter.Attributes["end"].ToDateTime(out end);
            else //if not then assign -infinite
                end = DateTime.MaxValue;

            ///Get the DTSTART of the component.
            var compDTSTART = component.GetComponentProperty("DTSTART") == null?
                DateTime.MaxValue: ((IValue<DateTime>) component.GetComponentProperty("DTSTART")).Value;

            ///if the component contains RRULES then expand the dts
            IEnumerable<DateTime> expandedDates = null;
            if (component.MultipleValuesProperties["RRULE"].Any())
                expandedDates = compDTSTART.ExpandTime(component.GetMultipleCompProperties("RRULE").Select(
                    x => ((IValue<Recur>) x).Value).ToList());
            if (expandedDates == null)
                expandedDates = new List<DateTime> {compDTSTART};
            
            
            if (component is VEvent)
                return component.ApplyTimeFilterToVEVENT(start.Value, end.Value, expandedDates);
            if (component is VTodo)
            {
                return component.ApplyTimeFilterToVTODO(start.Value, end.Value, expandedDates);
            }
            if (component is VJournal)
                throw new NotImplementedException("The doesn't support the VJOURNALs yet.");
            if (component is VFreebusy)
            {
                ///| VFREEBUSY has both the DTSTART and DTEND properties? Y
                /// VFREEBUSY has the FREEBUSY property? *
                
            }
            return false;
        }

        /// <summary>
        /// Apply the time-filter to the VTODOs components.
        ///A VTODO component overlaps a given time range if the condition for the
        ///corresponding component state specified in the table (pg64 RFC 4791) is satisfied.
        /// </summary>
        /// <param name="component">The VTODO</param>
        /// <param name="start">The start time of the filter</param>
        /// <param name="end">The end datetime of the filter</param>
        /// <param name="expandedDates">The expaned dts by the RRULEs if any.</param>
        /// <returns>True if pass the filter, false otherwise.</returns>
        private static bool ApplyTimeFilterToVTODO(this ICalendarComponent component, DateTime start, DateTime end,
            IEnumerable<DateTime> expandedDates)
        {
            DurationType DURATION = component.GetComponentProperty("DURATION") == null ?
                                   null : ((IValue<DurationType>)component.GetComponentProperty("DURATION")).Value;
            Due DUE = component.GetComponentProperty("DUE") == null ?
                                 null : ((IValue<Due>)component.GetComponentProperty("DUE")).Value; ;
            Completed COMPLETED = component.GetComponentProperty("COMPLETED") == null ?
                                null : ((IValue<Completed>)component.GetComponentProperty("COMPLETED")).Value; ;
            Created CREATED = component.GetComponentProperty("CREATED") == null ?
                                null : ((IValue<Created>)component.GetComponentProperty("Created")).Value; ;

            ///VTODO has the DTSTART property? Y
            if (expandedDates.Any())
            {
                foreach (var DTSTART in expandedDates)
                {
                    /// VTODO has the DURATION property? Y
                    /// VTODO has the DUE property? N
                    ///  VTODO has the COMPLETED property? *
                    /// VTODO has the CREATED property? *
                    if (DURATION != null && DUE == null)
                    {
                        var DTSTARTplusDURATION = DTSTART.AddDuration(DURATION);
                        if (start <= DTSTARTplusDURATION
                            && (end > DTSTART || end > DTSTARTplusDURATION))
                            return true;
                    }

                    /// VTODO has the DURATION property? N
                    /// VTODO has the DUE property? Y
                    ///  VTODO has the COMPLETED property? *
                    /// VTODO has the CREATED property? *
                    else if (DUE != null)
                    {
                        if ((start < DUE.Value || start <= DTSTART) &&
                            (end > DTSTART) || end >= DUE.Value)
                            return true;
                    }

                    /// VTODO has the DURATION property? N
                    /// VTODO has the DUE property? N
                    ///  VTODO has the COMPLETED property? *
                    /// VTODO has the CREATED property? *
                    if (DUE == null && DURATION == null)
                        if (start <= DTSTART && end > DTSTART)
                            return true;
                }
            }

            ///VTODO has the DTSTART property? N
            else
            {
                /// VTODO has the DURATION property? N
                /// VTODO has the DUE property? Y
                ///  VTODO has the COMPLETED property? *
                /// VTODO has the CREATED property? *
                if (DUE != null && DURATION == null)
                    if (start < DUE.Value && end >= DUE.Value)
                        return true;

                /// VTODO has the DURATION property? N
                /// VTODO has the DUE property? N
                ///  VTODO has the COMPLETED property? Y
                /// VTODO has the CREATED property? Y
                if (COMPLETED != null && CREATED != null)
                {
                    if ((start <= CREATED.Value || start <= COMPLETED.Value)
                        && end >= CREATED.Value || end >= COMPLETED.Value)
                        return true;
                }

                /// VTODO has the DURATION property? N
                /// VTODO has the DUE property? N
                ///  VTODO has the COMPLETED property? Y
                /// VTODO has the CREATED property? N
                else if (COMPLETED != null && CREATED == null)
                {
                    if (start <= COMPLETED.Value && end >= COMPLETED.Value)
                        return true;
                }

                /// VTODO has the DURATION property? N
                /// VTODO has the DUE property? N
                ///  VTODO has the COMPLETED property? N
                /// VTODO has the CREATED property? Y
                else if (COMPLETED == null && CREATED != null)
                    if (end > CREATED.Value)
                        return true;

               

            }
            /// VTODO has the DURATION property? N
            /// VTODO has the DUE property? N
            ///  VTODO has the COMPLETED property? N
            /// VTODO has the CREATED property? N
            return true;
        }

        /// <summary>
        /// Apply the time-filter to the VEVENT components.
        ///A VEVENT component overlaps a given time range if the condition for the
        ///corresponding component state specified in the table (pg64 RFC 4791) is satisfied.
        /// </summary>
        /// <param name="component">THe VTODO</param>
        /// <param name="start">The start time of the filter</param>
        /// <param name="end">The end datetime of the filter</param>
        /// <param name="expandedDates">The expaned dts by the RRULEs if any.</param>
        /// <returns>True if pass the filter, false otherwise.</returns>
        private static bool ApplyTimeFilterToVEVENT(this ICalendarComponent component, DateTime start, DateTime end,
            IEnumerable<DateTime> expandedDates)
        {
            var compEndTimeProp = component.GetComponentProperty("DTEND");
            foreach (var DTSTART in expandedDates)
            {
                ///VEVENT has the DTEND property? Y
                /// VEVENT has the DURATION property? N
                /// DURATION property value is greater than 0 seconds? N
                /// DTSTART property is a DATE-TIME value? *
                if (compEndTimeProp != null)
                {
                    var DTEND = ((IValue<DateTime>)compEndTimeProp).Value;
                    if (start < DTEND && end > DTSTART)
                        return true;
                }

                var durationProp = component.GetComponentProperty("DURATION");

                ///VEVENT has the DTEND property? N
                /// VEVENT has the DURATION property? Y
                if (durationProp != null)
                {
                    var DURATION = ((IValue<DurationType>)durationProp).Value;
                    var DTSTARTplusDURATION = DTSTART.AddDuration(DURATION);

                    /// DURATION property value is greater than 0 seconds? Y
                    /// DTSTART property is a DATE-TIME value? *
                    if (DURATION.IsPositive)
                    {
                        if (start < DTSTARTplusDURATION && end > DTSTART)
                            return true;
                    }
                    /// DURATION property value is greater than 0 seconds? N
                    /// DTSTART property is a DATE-TIME value? *
                    else
                    {
                        if (start <= DTSTART && end > DTSTART)
                            return true;
                    }
                }

                ///VEVENT has the DTEND property? N
                /// VEVENT has the DURATION property? N
                /// DURATION property value is greater than 0 seconds? N
                /// DTSTART property is a DATE-TIME value? Y
                if (start <= DTSTART && end > DTSTART)
                    return true;
               
                ///VEVENT has the DTEND property? N
                /// VEVENT has the DURATION property? N
                /// DURATION property value is greater than 0 seconds? N
                /// DTSTART property is a DATE-TIME value? N
                if (start < DTSTART.AddDays(1) && end > DTSTART)
                    return true;

            }
            return false;

        }

        /// <summary>
        /// Apply the time-filter to the VFREEBUSY components.
        ///A VFREEBUSY component overlaps a given time range if the condition for the
        ///corresponding component state specified in the table (pg66 RFC 4791) is satisfied.
        /// </summary>
        /// <param name="component">THe VTODO</param>
        /// <param name="start">The start time of the filter</param>
        /// <param name="end">The end datetime of the filter</param>
        /// <param name="expandedDates">The expaned dts by the RRULEs if any.</param>
        /// <returns>True if pass the filter, false otherwise.</returns>
        private static bool ApplyTimeFilterToVFREEBUSY(this ICalendarComponent component, DateTime start, DateTime end,
            IEnumerable<DateTime> expandedDates)
        {
            var DTSTART = component.GetComponentProperty("DTSTART") == null ?
                DateTime.MinValue : ((IValue<DateTime>)component.GetComponentProperty("DTSTART")).Value;
            var DTEND = component.GetComponentProperty("DTEND") == null ?
               DateTime.MaxValue : ((IValue<DateTime>)component.GetComponentProperty("DTEND")).Value;

            ///VFREEBUSY has both the DTSTART and DTEND properties? Y
            /// VFREEBUSY has the FREEBUSY property? *
            if (DTSTART != DateTime.MinValue && DTEND != DateTime.MaxValue)
                return start <= DTEND && end > DTSTART;

            ///VFREEBUSY has both the DTSTART and DTEND properties? N
            /// VFREEBUSY has the FREEBUSY property? Y
            if (component.MultipleValuesProperties.ContainsKey("FREEBUSY"))
            {
                ///take the freebusy properties
                var freeBProp = component.MultipleValuesProperties["FREEBUSY"]
                                .Select(x=> ((IValue<Period>)x).Value);

                if (freeBProp.Any(period => start < period.End.Value && end > period.Start.Value))
                    return true;
            }

            ///VFREEBUSY has both the DTSTART and DTEND properties? N
            /// VFREEBUSY has the FREEBUSY property? N
            return false;
        }


        /// <summary>
        ///     If the component doest specifies a DTEND so it should has a DURATION.
        ///     Use this to add the duration to the DTSTART property of a component.
        /// </summary>
        /// <param name="dtStart"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static DateTime AddDuration(this DateTime dtStart, DurationType duration)
        {
            if (duration.Weeks != null)
                if (duration.IsPositive)
                    return dtStart.AddDays(7*duration.Weeks.Value);
                else
                    return dtStart.Subtract(new TimeSpan(7*duration.Weeks.Value, 0, 0, 0));
            var durationSpan = new TimeSpan(
                duration.Days ?? 0,
                duration.Hours ?? 0,
                duration.Minutes ?? 0,
                duration.Seconds ?? 0);
            return duration.IsPositive ? dtStart.Add(durationSpan) : dtStart.Subtract(durationSpan);
        }

       
    }
}