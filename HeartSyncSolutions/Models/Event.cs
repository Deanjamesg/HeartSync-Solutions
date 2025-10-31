using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class Event
    {
        // Set the string ID in the constructor
        public Event()
        {
            EventID = Guid.NewGuid().ToString();
        }

        // [Key] tells EF that this is the Primary Key
        [Key]
        public string EventID { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public DateTime Date { get; set; }

        // This is the link to the EventType table
        public string EventTypeID { get; set; }

        // The 'virtual' keyword helps EF load this data later
        public virtual EventType EventType { get; set; }

        // This is the link to the EventStatus table
        public string EventStatusID { get; set; }

        public virtual EventStatus EventStatus { get; set; }

        // An Event can have many volunteers (UserEvents)
        public virtual ICollection<UserEvent> UserEvents { get; set; }

        // An Event can have many gallery images
        public virtual ICollection<EventGallery> GalleryImages { get; set; }

        // An Event has one report
        public virtual EventReport EventReport { get; set; }
    }
}