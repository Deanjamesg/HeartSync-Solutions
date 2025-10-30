using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // This just stores the links to all the pictures for an event.
    // So the admin can upload photos of the outreach.
    public class EventGallery
    {
        [Key] // The primary key
        public int EventGalleryID { get; set; }

        public string ImageURL { get; set; }

        public string Caption { get; set; }

        // This links this picture back to a specific event
        public int EventID { get; set; }

        public virtual Event Event { get; set; }
    }
}
