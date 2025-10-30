using HeartSyncSolutions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Services
{
    public interface IDonationService
    {
        // Monetary Donation Methods
        Task<MonetaryDonations> CreateMonetaryDonationAsync(MonetaryDonations donation);
        Task<MonetaryDonations> GetMonetaryDonationByIdAsync(int donationId);
        Task<IEnumerable<MonetaryDonations>> GetAllMonetaryDonationsAsync();
        Task<IEnumerable<MonetaryDonations>> GetMonetaryDonationsByUserAsync(string userId);
        Task<IEnumerable<MonetaryDonations>> GetAnonymousMonetaryDonationsAsync();
        Task<IEnumerable<MonetaryDonations>> GetMonetaryDonationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<double> GetTotalMonetaryDonationsAsync();
        Task<double> GetTotalMonetaryDonationsByUserAsync(string userId);
        Task<bool> UpdateMonetaryDonationAsync(MonetaryDonations donation);
        Task<bool> DeleteMonetaryDonationAsync(int donationId);

        // In-Kind Offer Methods
        Task<InKindOffer> CreateInKindOfferAsync(InKindOffer offer);
        Task<InKindOffer> GetInKindOfferByIdAsync(int offerId);
        Task<IEnumerable<InKindOffer>> GetAllInKindOffersAsync();
        Task<IEnumerable<InKindOffer>> GetInKindOffersByStatusAsync(string status);
        Task<IEnumerable<InKindOffer>> GetInKindOffersByUserAsync(string userId);
        Task<IEnumerable<InKindOffer>> GetInKindOffersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdateInKindOfferStatusAsync(int offerId, string newStatus);
        Task<bool> UpdateInKindOfferAsync(InKindOffer offer);
        Task<bool> DeleteInKindOfferAsync(int offerId);

        // Combined/Statistical Methods
        Task<int> GetTotalDonationCountAsync();
        Task<Dictionary<string, int>> GetDonationStatisticsByTypeAsync();
        Task<IEnumerable<MonetaryDonations>> GetRecentMonetaryDonationsAsync(int count);
        Task<IEnumerable<InKindOffer>> GetRecentInKindOffersAsync(int count);
    }
}