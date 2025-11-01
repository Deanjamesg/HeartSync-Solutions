using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class MonetaryDonationStatus
    {
        public MonetaryDonationStatus()
        {
            MonetaryDonationStatusID = Guid.NewGuid().ToString();
        }

        [Key] // Tells EF this is the primary key
        public string MonetaryDonationStatusID { get; set; }

        public string Status { get; set; }

        // An event status can be used by many events
        public virtual ICollection<MonetaryDonation> MonetaryDonations { get; set; }
    }
}
