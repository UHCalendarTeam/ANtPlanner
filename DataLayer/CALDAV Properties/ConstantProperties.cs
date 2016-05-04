namespace DataLayer
{
    /// <summary>
    ///     Class to declate the different constant values of the CAlDav server.
    /// </summary>
    public class ConstantsProperties
    {
        /// <summary>
        ///     Provides a numeric value indicating the maximum number of recurrence instances that a
        ///     calendar object resource stored in a calendar collection can generate.
        /// </summary>
        public int MaxIntances => 25;

        /// <summary>
        ///     max-date-time
        /// </summary>
        public string MaxDateTime => "date here"; //TODO: set this value

        /// <summary>
        ///     max-date-time
        /// </summary>
        public string MinDateTime => "date here"; //TODO: set this value

        /// <summary>
        ///     max-resource-size
        /// </summary>
        public int MaxResourceSize => 1024;

        /// <summary>
        ///     calendar time zone
        /// </summary>
        public string CalendarTimeZone => "set the value";
    }
}