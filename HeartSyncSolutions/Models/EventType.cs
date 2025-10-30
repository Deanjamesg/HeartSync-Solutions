using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // Class that holds the options for the type of event it is e.g. "Food Drive".
    // Simple class that we call a 'lookup table'.
    public class EventType
    {
        [Key] // Tells EF this is the primary key
        public int Event_Type_ID { get; set; }

        public string Title { get; set; }

        // This line links it back to the Event table.
        public virtual ICollection<Event> Events { get; set; }
    }
}
