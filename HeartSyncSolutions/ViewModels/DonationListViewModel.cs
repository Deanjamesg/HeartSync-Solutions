using System.Collections.Generic;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// View model for displaying all donations (admin view)
    /// </summary>
    public class DonationListViewModel
    {
        public IEnumerable<MonetaryDonationViewModel> MonetaryDonations { get; set; } = new List<MonetaryDonationViewModel>();

        public IEnumerable<InKindDonationViewModel> InKindDonations { get; set; } = new List<InKindDonationViewModel>();

        public double TotalMonetaryAmount { get; set; }

        public int TotalMonetaryCount { get; set; }

        public int TotalInKindCount { get; set; }

        public int TotalDonationCount { get; set; }

        public string FilterStatus { get; set; }

        public string FilterType { get; set; }

        public string SortBy { get; set; }
    }
}