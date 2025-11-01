using System;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // This table logs all one-time donations
    public class MonetaryDonations
    {
        // Set the string ID in the constructor
        public MonetaryDonations()
        {
            MonetaryDonationID = Guid.NewGuid().ToString();
        }

        [Key] // The primary key
        public string MonetaryDonationID { get; set; }

        public double DonationAmount { get; set; }

        public DateTime Date { get; set; }

        public bool IsAnonymous { get; set; } = false;

        // This is so someone who isn't logged in can still donate.
        public string? ApplicationUserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
