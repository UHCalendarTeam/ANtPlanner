namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     Represent a user in the system that
    ///     is a worker. That means that the
    ///     professors are gonna be Workers
    /// </summary>
    public class Worker : User
    {
        public Worker()
        {
            
        }

        public Worker(string displayName, string email, string password,
            string deparment, string faculty): base(displayName,email, password)
        {
            Deparment = deparment;
            Faculty = faculty;
        }

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