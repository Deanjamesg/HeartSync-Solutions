using System.Collections.Generic;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// View model for donation statistics and analytics
    /// </summary>
    public class DonationStatisticsViewModel
    {
        public double TotalMonetaryDonations { get; set; }

        public int TotalMonetaryDonationCount { get; set; }

        public int TotalInKindDonationCount { get; set; }

        public int TotalDonationCount { get; set; }

        public double AverageDonationAmount { get; set; }

        public double LargestDonation { get; set; }

        public int TotalDonors { get; set; }

        public int AnonymousDonationCount { get; set; }

        public Dictionary<string, int> DonationsByMonth { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, double> MonetaryDonationsByMonth { get; set; } = new Dictionary<string, double>();

        public Dictionary<string, int> InKindDonationsByCategory { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> InKindDonationsByStatus { get; set; } = new Dictionary<string, int>();

        public IEnumerable<MonetaryDonationViewModel> RecentMonetaryDonations { get; set; } = new List<MonetaryDonationViewModel>();

        public IEnumerable<InKindDonationViewModel> RecentInKindDonations { get; set; } = new List<InKindDonationViewModel>();
    }
}