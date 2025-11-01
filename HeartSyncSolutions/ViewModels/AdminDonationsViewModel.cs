using HeartSyncSolutions.Models;

namespace HeartSyncSolutions.ViewModels
{
    public class AdminDonationsViewModel
    {
        public List<MonetaryDonation> MonetaryDonations { get; set; } = new List<MonetaryDonation>();
        public List<InKindDonation> InKindDonations { get; set; } = new List<InKindDonation>();

        public double TotalMonetaryReceived =>
            MonetaryDonations
                .Where(d => d.MonetaryDonationStatus?.Status == "Completed")
                .Sum(d => d.DonationAmount);

        public int TotalDonationsCount =>
            MonetaryDonations.Count + InKindDonations.Count;

        public int PendingDonationsCount =>
            MonetaryDonations.Count(d => d.MonetaryDonationStatus?.Status == "Pending") +
            InKindDonations.Count(d => d.InKindStatus?.Status == "Pending");
    }
}
