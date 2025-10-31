using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // Simple lookup table for a drop down menu on admin dashboard
    // e.g. "Up Coming" , "Active" , "Completed".
    public class EventStatus
    {
        // Set the string ID in the constructor
        public EventStatus()
        {
            EventStatusID = Guid.NewGuid().ToString();
        }

        [Key] // Tells EF this is the primary key
        public string EventStatusID { get; set; }

        public string Status { get; set; }

        // An event status can be used by many events
        public virtual ICollection<Event> Events { get; set; }
    }
}