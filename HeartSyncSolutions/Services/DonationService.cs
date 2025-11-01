using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions.Services
{
    public class DonationService
    {
        private readonly ApplicationDbContext _context;

        public DonationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get User's Monetary Donations
        public async Task<List<MonetaryDonation>> GetUserMonetaryDonationsAsync(string userId)
        {
            return await _context.MonetaryDonations
                .Include(md => md.MonetaryDonationStatus)
                .Where(md => md.UserID == userId)
                .OrderByDescending(md => md.Date)
                .ToListAsync();
        }

        // Get User's In-Kind Donations
        public async Task<List<InKindDonation>> GetUserInKindDonationsAsync(string userId)
        {
            return await _context.InKindDonations
                .Include(ikd => ikd.InKindStatus)
                .Where(ikd => ikd.UserID == userId)
                .OrderByDescending(ikd => ikd.DeliveryDate)
                .ToListAsync();
        }
    }
}