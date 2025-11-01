using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    // This table stores the subscription agreement.
    public class PartnerPledge
    {

        // Set the string ID in the constructor
        public PartnerPledge()
        {
            PartnerPledgeID = Guid.NewGuid().ToString();
        }

        [Key] // The primary key
        public string PartnerPledgeID { get; set; }

        public double MonthlyAmount { get; set; }

        public string Status { get; set; } // e.g. "Active" , "Cancelled".

        // This will store the subscription ID or token from PayFast 
        // so we can identify it later.
        public string PayFastToken { get; set; }

        // This is the user who owns this pledge
        public string UserID { get; set; }

        public virtual User User { get; set; }
    }
}
