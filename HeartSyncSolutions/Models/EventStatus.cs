using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{       
    // Simple lookup table for a drop down menu on admin dashboard to hold "Planning" , "Active" , "Completed".
    public class EventStatus
    {
        [Key] // Tells EF this is the primary key
        public int EventStatusID { get; set; }

        public string Status { get; set; }

        // An event status can be used by many events
        public virtual ICollection<Event> Events { get; set; }
    }
}
