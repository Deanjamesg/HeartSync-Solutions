using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class UserEvent
    {
        // Set the string ID in the constructor
        public UserEvent()
        {
            UserEventID = Guid.NewGuid().ToString();
        }

        [Key] // The primary key for this table
        public string UserEventID { get; set; }

        // This links to the 'Events' table
        public string EventID { get; set; }

        public virtual Event Event { get; set; }

        public string UserID { get; set; }

        public virtual User User { get; set; }

        // This links to the "Attended" / "Not Attended" status
        public string AttendanceStatusID { get; set; }

        public virtual AttendanceStatus AttendanceStatus { get; set; }
    }
}
