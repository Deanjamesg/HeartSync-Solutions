using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeartSyncSolutions.Models
{
    // The admin will write a summary of how the event went.
    // and this will be displayed on the public website.
    public class EventReport
    {
        // We make the Primary Key (PK) also the Foreign Key (FK) for a 1-to-1 relationship.
        [Key]
        [ForeignKey("Event")]

        public int EventID { get; set; }

        // This is where the admin writes the report summary
        public string Summary { get; set; }

        // This links it back to the Event
        public virtual Event Event { get; set; }
    }
}
