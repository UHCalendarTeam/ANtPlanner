using System.ComponentModel.DataAnnotations;

namespace CalDAV.Models
{
    /// <summary>
    /// To store the url of a user's collection
    /// </summary>
    public class CalendarCollection
    {
        public int CalendarCollectionId { get; set; }

        [Required]
        public string Url { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}