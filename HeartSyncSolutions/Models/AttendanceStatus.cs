using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // Lookup table for our volunteer print-out.
    // e.g. "Signed up" , "Attended" , "No Show".
    public class AttendanceStatus
    {
        // Set the string ID in the constructor
        public AttendanceStatus()
        {
            AttendanceStatusID = Guid.NewGuid().ToString();
        }

        [Key] // Tells EF this is the primary key.
        public string AttendanceStatusID { get; set; }

        public string Status { get; set; }

        // This links to the UserEvent table.
        public virtual ICollection<UserEvent> UserEvents { get; set; }
    }
}
