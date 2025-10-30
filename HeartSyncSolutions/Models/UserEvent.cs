using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // This will be used as a linking table for our many-to-many relationships.
    // It Links Users to an Event.
    public class UserEvent
    {
        [Key] // The primary key for this table
        public int User_Event_ID { get; set; }

        // This links to the 'Events' table
        public int Event_ID { get; set; }

        public virtual Event Event { get; set; }

        public string ApplicationUser_ID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        // This links to the "Attended" / "Not Attended" status
        public int Attendance_Status_ID { get; set; }

        public virtual AttendanceStatus AttendanceStatus { get; set; }
    }
}
