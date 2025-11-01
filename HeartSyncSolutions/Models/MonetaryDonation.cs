using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class MonetaryDonation
    {
        // Set the string ID in the constructor
        public MonetaryDonation()
        {
            MonetaryDonationID = Guid.NewGuid().ToString();
        }

        [Key] // The primary key
        public string MonetaryDonationID { get; set; }

        public double DonationAmount { get; set; }

        public DateTime Date { get; set; }

        // "Pending", "Completed", "Cancelled"
        public string MonetaryDonationStatusID { get; set; }

        public virtual MonetaryDonationStatus MonetaryDonationStatus { get; set; }

        public string UserID { get; set; }

        public virtual User User { get; set; }
    }
}
