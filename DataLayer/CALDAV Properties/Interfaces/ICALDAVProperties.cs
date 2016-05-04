namespace DataLayer
{
    public interface ICALDAVProperties
    {
        /// <summary>
        ///     Identifies the set of collations supported by
        ///     the server for text matching operations.
        /// </summary>
        /// <returns></returns>
        string SupportedCollationSet();
    }
}