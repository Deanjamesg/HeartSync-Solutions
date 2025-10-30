using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // Lookup table for our volunteer print-out.
    // e.g. "Signed up" , "Attented" , "No Show".
    public class AttendanceStatus
    {
            [Key] // Tells EF this is the primary key.
            public int Attendance_Status_ID { get; set; }

            public string Status { get; set; }

            // This links to the User_Event table.
            public virtual ICollection<UserEvent> UserEvents { get; set; }
        
    }
}
