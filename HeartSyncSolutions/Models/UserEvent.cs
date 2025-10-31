using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // This will be used as a linking table for our many-to-many relationships.
    // It Links Users to an Event.
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

        public string ApplicationUserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        // This links to the "Attended" / "Not Attended" status
        public string AttendanceStatusID { get; set; }

        public virtual AttendanceStatus AttendanceStatus { get; set; }
    }
}
