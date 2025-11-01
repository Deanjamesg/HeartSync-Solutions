using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class InKindDonation
    {
        // Set the string ID in the constructor
        public InKindDonation()
        {
            InKindDonationID = Guid.NewGuid().ToString();
        }

        [Key] // The primary key
        public string InKindDonationID { get; set; }

        // This is what they're offering, e.g. "2 bags of clothes".
        public string ItemDescription { get; set; }

        // Admin will update this, e.g. "Pending", "Approved", "Completed", "Cancelled".
        public string InKindStatusID { get; set; }

        public virtual InKindStatus InKindStatus { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string UserID { get; set; }

        public virtual User User { get; set; }
    }
}
