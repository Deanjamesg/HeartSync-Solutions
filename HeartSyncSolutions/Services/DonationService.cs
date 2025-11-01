using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Services
{
    public class DonationService : IDonationService
    {
        private readonly ApplicationDbContext _context;

        public DonationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MonetaryDonations> CreateMonetaryDonationAsync(MonetaryDonations donation)
        {
            if (donation == null)
                throw new ArgumentNullException(nameof(donation));

            // Set the date if not already set
            if (donation.Date == default)
                donation.Date = DateTime.Now;

            _context.MonetaryDonations.Add(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        public async Task<MonetaryDonations> GetMonetaryDonationByIdAsync(string donationId)
        {
            if (string.IsNullOrWhiteSpace(donationId))
                return null;

            return await _context.MonetaryDonations
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.MonetaryDonationID == donationId);
        }

        public async Task<IEnumerable<MonetaryDonations>> GetAllMonetaryDonationsAsync()
        {
            return await _context.MonetaryDonations
                .Include(d => d.ApplicationUser)
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonetaryDonations>> GetMonetaryDonationsByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<MonetaryDonations>();

            return await _context.MonetaryDonations
                .Include(d => d.ApplicationUser)
                .Where(d => d.ApplicationUserID == userId)
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonetaryDonations>> GetAnonymousMonetaryDonationsAsync()
        {
            return await _context.MonetaryDonations
                .Where(d => d.IsAnonymous == true)
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonetaryDonations>> GetMonetaryDonationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MonetaryDonations
                .Include(d => d.ApplicationUser)
                .Where(d => d.Date >= startDate && d.Date <= endDate)
                .OrderBy(d => d.Date)
                .ToListAsync();
        }

        public async Task<double> GetTotalMonetaryDonationsAsync()
        {
            var total = await _context.MonetaryDonations.SumAsync(d => d.DonationAmount);
            return total;
        }

        public async Task<double> GetTotalMonetaryDonationsByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return 0;

            var total = await _context.MonetaryDonations
                .Where(d => d.ApplicationUserID == userId)
                .SumAsync(d => d.DonationAmount);
            return total;
        }

        public async Task<bool> UpdateMonetaryDonationAsync(MonetaryDonations donation)
        {
            if (donation == null)
                throw new ArgumentNullException(nameof(donation));

            var existingDonation = await _context.MonetaryDonations.FindAsync(donation.MonetaryDonationID);
            if (existingDonation == null)
                return false;

            existingDonation.DonationAmount = donation.DonationAmount;
            existingDonation.Date = donation.Date;
            existingDonation.IsAnonymous = donation.IsAnonymous;
            existingDonation.ApplicationUserID = donation.ApplicationUserID;

            _context.MonetaryDonations.Update(existingDonation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMonetaryDonationAsync(string donationId)
        {
            if (string.IsNullOrWhiteSpace(donationId))
                return false;

            var donation = await _context.MonetaryDonations.FindAsync(donationId);
            if (donation == null)
                return false;

            _context.MonetaryDonations.Remove(donation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InKindOffer> CreateInKindOfferAsync(InKindOffer offer)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            // Set the date if not already set
            if (offer.OfferedDate == default)
                offer.OfferedDate = DateTime.Now;

            // Set default status if not provided
            if (string.IsNullOrWhiteSpace(offer.Status))
                offer.Status = "New Offer";

            _context.InKindOffers.Add(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public async Task<InKindOffer> GetInKindOfferByIdAsync(string offerId)
        {
            if (string.IsNullOrWhiteSpace(offerId))
                return null;

            return await _context.InKindOffers
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(o => o.InKindOfferID == offerId);
        }

        public async Task<IEnumerable<InKindOffer>> GetAllInKindOffersAsync()
        {
            return await _context.InKindOffers
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OfferedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InKindOffer>> GetInKindOffersByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return new List<InKindOffer>();

            return await _context.InKindOffers
                .Include(o => o.ApplicationUser)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OfferedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InKindOffer>> GetInKindOffersByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<InKindOffer>();

            return await _context.InKindOffers
                .Include(o => o.ApplicationUser)
                .Where(o => o.ApplicationUserID == userId)
                .OrderByDescending(o => o.OfferedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InKindOffer>> GetInKindOffersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.InKindOffers
                .Include(o => o.ApplicationUser)
                .Where(o => o.OfferedDate >= startDate && o.OfferedDate <= endDate)
                .OrderBy(o => o.OfferedDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateInKindOfferStatusAsync(string offerId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus) || string.IsNullOrWhiteSpace(offerId))
                return false;

            var offer = await _context.InKindOffers.FindAsync(offerId);
            if (offer == null)
                return false;

            offer.Status = newStatus;
            _context.InKindOffers.Update(offer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateInKindOfferAsync(InKindOffer offer)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            var existingOffer = await _context.InKindOffers.FindAsync(offer.InKindOfferID);
            if (existingOffer == null)
                return false;

            existingOffer.ItemDescription = offer.ItemDescription;
            existingOffer.Status = offer.Status;
            existingOffer.OfferedDate = offer.OfferedDate;
            existingOffer.ApplicationUserID = offer.ApplicationUserID;

            _context.InKindOffers.Update(existingOffer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInKindOfferAsync(string offerId)
        {
            if (string.IsNullOrWhiteSpace(offerId))
                return false;

            var offer = await _context.InKindOffers.FindAsync(offerId);
            if (offer == null)
                return false;

            _context.InKindOffers.Remove(offer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalDonationCountAsync()
        {
            var monetaryCount = await _context.MonetaryDonations.CountAsync();
            var inKindCount = await _context.InKindOffers.CountAsync();
            return monetaryCount + inKindCount;
        }

        public async Task<Dictionary<string, int>> GetDonationStatisticsByTypeAsync()
        {
            var monetaryCount = await _context.MonetaryDonations.CountAsync();
            var inKindCount = await _context.InKindOffers.CountAsync();

            return new Dictionary<string, int>
            {
                { "Monetary", monetaryCount },
                { "In-Kind", inKindCount }
            };
        }

        public async Task<IEnumerable<MonetaryDonations>> GetRecentMonetaryDonationsAsync(int count)
        {
            if (count <= 0)
                count = 10;

            return await _context.MonetaryDonations
                .Include(d => d.ApplicationUser)
                .OrderByDescending(d => d.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<InKindOffer>> GetRecentInKindOffersAsync(int count)
        {
            if (count <= 0)
                count = 10;

            return await _context.InKindOffers
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OfferedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}