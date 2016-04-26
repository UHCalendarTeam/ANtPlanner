namespace DataLayer
{
    /// <summary>
    ///     Contains some useful properties for the system.
    ///     Declare here the appSettings.
    /// </summary>
    public static class SystemProperties
    {
        /// <summary>
        ///     A principal represent either a group or
        ///     a user.
        /// </summary>
        public enum PrincipalType
        {
            Group,
            Student,
            Worker,
            User
        }

        /// <summary>
        /// Contains the base url path for the system.
        /// </summary>
        public static readonly string _baseUrl = "/api/v1/caldav";

        /// <summary>
        ///     Contains the url for the user's collections
        ///     Add the email of the user
        /// </summary>
        public static readonly string _userCollectionUrl = _baseUrl + "/collections/users/";

        /// <summary>
        ///     Contains the url for the groups collection.
        ///     Add the name of the group
        /// </summary>
        public static readonly string _groupCollectionUrl = _baseUrl + "/collections/groups/";

        /// <summary>
        ///     Contains the default name for the user collections
        /// </summary>
        public static readonly string _defualtInitialCollectionName = "DefaultCalendar";

        public static readonly string _principalUrl = "principals";

        /// <summary>
        ///     Contains the url that has to be assigned the the priciapl that
        ///     represent a normal user.
        ///     The email of the user has to be added to the end of the url
        /// </summary>
        public static readonly string _userPrincipalUrl = _baseUrl + "/principals/users/";

        /// <summary>
        ///     Contains the url that has to be assigned the the priciapl that
        ///     represent a group.
        ///     The name of the group has to be added.
        /// </summary>
        public static readonly string _groupPrincipalUrl = _baseUrl + "/principals/groups/";

        /// <summary>
        ///     Build the url that identify the collection where are the
        ///     calendar of principal.
        /// </summary>
        /// <param name="type">A principal can represent either a user or group.</param>
        /// <param name="principalId">
        ///     If the principal represents a user then put ith email here
        ///     otherwise put the name of the group here.
        /// </param>
        /// <returns>The url where to find the calendars of the principal.</returns>
        public static string BuildHomeSetUrl(PrincipalType type, string principalId)
        {
            ///take the beginning of the url depending of the king of principal
            var colUrl = type == PrincipalType.User ? _userCollectionUrl : _groupCollectionUrl;

            //add the identifier of the pricipal
            return $"{colUrl}{principalId}";
        }
    }
}