using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HeartSyncSolutions.Models
{
    // This is our custom User class. It's inheriting from IdentityUser
    //EF will add these columns to the 'ASPNetUsers' table.
    public class ApplicationUser : IdentityUser
    {
        // These are our custom fields
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactNumber { get; set; }

        // We can use these bools to check what kind of user they are
        public bool IsVolunteer { get; set; }

        public bool IsDonor { get; set; }

        // This links to the User_Event table.
        // A user can sign up for lots of events.
        public virtual ICollection<UserEvent> UserEvents { get; set; }

        // A user can make lots of donations
        public virtual ICollection<MonetaryDonations> MonetaryDonations { get; set; }

        // A user can make lots of in-kind offers
        public virtual ICollection<InKindOffer> InKindOffers { get; set; }

        // A user can have one pledge. This is a 1-to-1 link.
        public virtual PartnerPledge PartnerPledge { get; set; }
    }
}
