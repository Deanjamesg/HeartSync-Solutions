using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeartSyncSolutions.Models
{
    // This table stores the subscription agreement.
    public class PartnerPledge
    {
        [Key] // The primary key
        public int PartnerPledgeID { get; set; }

        public double MonthlyAmount { get; set; }

        public string Status { get; set; } // e.g. "Active" , "Cancelled".

        // This will store the subscription ID or token from PayFast 
        // so we can identify it later.
        public string PayFastToken { get; set; }

        // This is the user who owns this pledge
        public string ApplicationUserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
