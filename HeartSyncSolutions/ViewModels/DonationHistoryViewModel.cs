using HeartSyncSolutions.Models;

namespace HeartSyncSolutions.ViewModels
{
    public class DonationHistoryViewModel
    {
        public double TotalDonated { get; set; }
        public int TotalDonations { get; set; }
        public double MonthlyRecurring { get; set; }
        public int InKindDonationsCount { get; set; }

        public List<MonetaryDonation> MonetaryDonations { get; set; } = new List<MonetaryDonation>();
        public List<InKindDonation> InKindDonations { get; set; } = new List<InKindDonation>();

        public List<DonationTimelineItem> GetTimelineItems()
        {
            var items = new List<DonationTimelineItem>();

            foreach (var donation in MonetaryDonations)
            {
                items.Add(new DonationTimelineItem
                {
                    Type = "Monetary",
                    Date = donation.Date,
                    Amount = donation.DonationAmount,
                    Description = "Monetary Donation",
                    Status = donation.MonetaryDonationStatus?.Status ?? "Unknown",
                    Id = donation.MonetaryDonationID
                });
            }

            foreach (var donation in InKindDonations)
            {
                items.Add(new DonationTimelineItem
                {
                    Type = "In-Kind",
                    Date = donation.DeliveryDate,
                    Amount = null,
                    Description = donation.ItemDescription,
                    Status = donation.InKindStatus?.Status ?? "Unknown",
                    Id = donation.InKindDonationID
                });
            }

            return items.OrderByDescending(i => i.Date).ToList();
        }

        public Dictionary<string, List<DonationTimelineItem>> GetItemsGroupedByMonth()
        {
            var grouped = new Dictionary<string, List<DonationTimelineItem>>();
            var items = GetTimelineItems();

            foreach (var item in items)
            {
                var monthYear = item.Date.ToString("MMMM yyyy");

                if (!grouped.ContainsKey(monthYear))
                {
                    grouped[monthYear] = new List<DonationTimelineItem>();
                }

                grouped[monthYear].Add(item);
            }

            return grouped;
        }
    }

    public class DonationTimelineItem
    {
        public string Id { get; set; }
        public string Type { get; set; } // "Monetary" or "In-Kind"
        public DateTime Date { get; set; }
        public double? Amount { get; set; } // Nullable for in-kind donations
        public string Description { get; set; }
        public string Status { get; set; }
    }

}
