using System;
using System.Collections.Generic;

namespace HeartSyncSolutions.ViewModels
{
    /// <summary>
    /// View model for displaying a user's donation history
    /// </summary>
    public class DonationHistoryViewModel
    {
        public string UserId { get; set; }
        
        public string UserName { get; set; }

        public IEnumerable<MonetaryDonationViewModel> MonetaryDonations { get; set; } = new List<MonetaryDonationViewModel>();

        public IEnumerable<InKindDonationViewModel> InKindDonations { get; set; } = new List<InKindDonationViewModel>();

        public double TotalMonetaryDonations { get; set; }

        public int TotalInKindDonations { get; set; }

        public int TotalDonationCount { get; set; }

        public DateTime? FirstDonationDate { get; set; }

        public DateTime? LastDonationDate { get; set; }
    }
}