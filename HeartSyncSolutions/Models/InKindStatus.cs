using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class InKindStatus
    {
        public InKindStatus()
        {
            InKindStatusID = Guid.NewGuid().ToString();
        }

        [Key] // Tells EF this is the primary key
        public string InKindStatusID { get; set; }

        // "Pending", "Approved", "Completed", "Cancelled".
        public string Status { get; set; }

        public virtual ICollection<InKindDonation> InKindDonation { get; set; }
    }
}
