using Microsoft.AspNetCore.Identity;

namespace HeartSyncSolutions.Models
{
    public class User : IdentityUser
    {
        // These are our custom fields
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactNumber { get; set; }

        // We can use these bools to check what kind of user they are
        public bool IsVolunteer { get; set; } = false;

        public bool IsDonor { get; set; } = false;

        // This links to the UserEvent table.
        // A user can sign up for lots of events.
        public virtual ICollection<UserEvent> UserEvents { get; set; }

        // A user can make many donations
        public virtual ICollection<MonetaryDonation> MonetaryDonations { get; set; }

        // A user can make many in-kind donations
        public virtual ICollection<InKindDonation> InKindDonations { get; set; }

    }
}
