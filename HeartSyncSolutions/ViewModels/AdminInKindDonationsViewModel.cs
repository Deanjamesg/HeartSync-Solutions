using HeartSyncSolutions.Models;

namespace HeartSyncSolutions.ViewModels
{
    public class AdminInKindDonationsViewModel
    {
        public List<InKindDonationItem> InKindDonations { get; set; } = new List<InKindDonationItem>();

        public int TotalInKindDonations => InKindDonations.Count;

        public int PendingCount => InKindDonations.Count(d => d.Status == "Pending");

        public int ApprovedCount => InKindDonations.Count(d => d.Status == "Approved");

        public int CompletedCount => InKindDonations.Count(d => d.Status == "Completed");

        public int CancelledCount => InKindDonations.Count(d => d.Status == "Cancelled");

        public List<InKindDonationItem> GetDonationsByStatus(string status)
        {
            return InKindDonations
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.DeliveryDate)
                .ToList();
        }

        public List<InKindDonationItem> GetUpcomingDeliveries()
        {
            return InKindDonations
                .Where(d => d.DeliveryDate >= DateTime.Today && 
                           (d.Status == "Pending" || d.Status == "Approved"))
                .OrderBy(d => d.DeliveryDate)
                .ToList();
        }
    }

    public class InKindDonationItem
    {
        public string InKindDonationID { get; set; }
        public string ItemDescription { get; set; }
        public string Status { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }
        public string DonorContactNumber { get; set; }
        
        // Helper properties for UI display
        public bool IsPending => Status == "Pending";
        public bool IsApproved => Status == "Approved";
        public bool IsCompleted => Status == "Completed";
        public bool IsCancelled => Status == "Cancelled";
        public bool IsOverdue => DeliveryDate < DateTime.Today && !IsCompleted && !IsCancelled;

        public static InKindDonationItem FromInKindDonation(InKindDonation donation)
        {
            return new InKindDonationItem
            {
                InKindDonationID = donation.InKindDonationID,
                ItemDescription = donation.ItemDescription,
                Status = donation.InKindStatus?.Status ?? "Unknown",
                DeliveryDate = donation.DeliveryDate,
                DonorName = donation.User != null 
                    ? $"{donation.User.FirstName} {donation.User.LastName}" 
                    : "Unknown Donor",
                DonorEmail = donation.User?.Email ?? "N/A",
                DonorContactNumber = donation.User?.ContactNumber ?? "N/A"
            };
        }
    }
}