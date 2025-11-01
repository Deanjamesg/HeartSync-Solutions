using System;
using System.ComponentModel.DataAnnotations;

namespace HeartSyncSolutions.Models
{
    public class InKindOffer
    {
        // Set the string ID in the constructor
        public InKindOffer()
        {
            InKindOfferID = Guid.NewGuid().ToString();
        }

        [Key] // The primary key
        public string InKindOfferID { get; set; }

        // This is what they're offering, e.g. "2 bags of clothes".
        public string ItemDescription { get; set; }

        // Admin will update this, e.g. "New Offer", "Contacted", "Collected".
        public string Status { get; set; }

        public DateTime OfferedDate { get; set; }


        // Nullable, in case a non-logged-in user makes an offer
        public string? ApplicationUserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}