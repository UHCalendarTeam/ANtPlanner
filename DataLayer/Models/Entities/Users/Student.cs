namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     Represent a user in the system that
    ///     is a student.
    /// </summary>
    public class Student : User
    {
        /// <summary>
        ///     The user career.
        /// </summary>
        public string Career { get; set; }

        /// <summary>
        ///     The student group where he belongs.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     The user year.
        /// </summary>
        public int Year { get; set; }
    }
}