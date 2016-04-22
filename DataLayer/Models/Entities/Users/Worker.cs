namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     Represent a user in the system that
    ///     is a worker. That means that the
    ///     professors are gonna be Workers
    /// </summary>
    public class Worker : User
    {
        /// <summary>
        ///     The professor department.
        /// </summary>
        public string Deparment { get; set; }

        /// <summary>
        ///     The professor faculty.
        /// </summary>
        public string Faculty { get; set; }
    }
}