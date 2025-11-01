using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using HeartSyncSolutions.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions.Services
{
    public class DonationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DonationService> _logger;

        public DonationService(ApplicationDbContext context, ILogger<DonationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Create Monetary Donation
        public async Task<bool> CreateMonetaryDonationAsync(MonetaryDonationViewModel model, string userId)
        {
            try
            {
                // Get the "Pending" status
                var pendingStatus = await _context.MonetaryDonationStatuses
                    .FirstOrDefaultAsync(s => s.Status == "Pending");

                if (pendingStatus == null)
                {
                    _logger.LogError("Pending status not found in MonetaryDonationStatuses");
                    return false;
                }

                // Create the monetary donation entity
                var donation = new MonetaryDonation
                {
                    DonationAmount = model.Amount,
                    Date = DateTime.Now,
                    UserID = userId,
                    MonetaryDonationStatusID = pendingStatus.MonetaryDonationStatusID
                };

                // Add to database
                _context.MonetaryDonations.Add(donation);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Monetary donation created successfully. ID: {donation.MonetaryDonationID}, Amount: R{model.Amount}, UserID: {userId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating monetary donation for user {userId}");
                return false;
            }
        }

        // Create In-Kind Donation
        public async Task<bool> CreateInKindDonationAsync(InKindDonationViewModel model, string userId)
        {
            try
            {
                // Get the "Pending" status
                var pendingStatus = await _context.InKindStatuses
                    .FirstOrDefaultAsync(s => s.Status == "Pending");

                if (pendingStatus == null)
                {
                    _logger.LogError("Pending status not found in InKindStatuses");
                    return false;
                }

                // Create the in-kind donation entity
                var donation = new InKindDonation
                {
                    ItemDescription = model.Description,
                    DeliveryDate = model.AvailableDate,
                    UserID = userId,
                    InKindStatusID = pendingStatus.InKindStatusID
                };

                // Add to database
                _context.InKindDonations.Add(donation);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"In-kind donation created successfully. ID: {donation.InKindDonationID}, UserID: {userId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating in-kind donation for user {userId}");
                return false;
            }
        }

        // Get User's Monetary Donations
        public async Task<List<MonetaryDonation>> GetUserMonetaryDonationsAsync(string userId)
        {
            return await _context.MonetaryDonations
                .Include(md => md.MonetaryDonationStatus)
                .Include(md => md.User)
                .Where(md => md.UserID == userId)
                .OrderByDescending(md => md.Date)
                .ToListAsync();
        }

        // Get User's In-Kind Donations
        public async Task<List<InKindDonation>> GetUserInKindDonationsAsync(string userId)
        {
            return await _context.InKindDonations
                .Include(ikd => ikd.InKindStatus)
                .Include(ikd => ikd.User)
                .Where(ikd => ikd.UserID == userId)
                .OrderByDescending(ikd => ikd.DeliveryDate)
                .ToListAsync();
        }

        // Get User's Donation Statistics
        public async Task<DonationStatistics> GetUserDonationStatisticsAsync(string userId)
        {
            try
            {
                // Get all monetary donations for the user
                var monetaryDonations = await _context.MonetaryDonations
                    .Where(md => md.UserID == userId)
                    .ToListAsync();

                // Calculate total in memory to avoid EF casting issues
                var totalMonetaryDonated = monetaryDonations.Sum(md => md.DonationAmount);
                var totalMonetaryDonations = monetaryDonations.Count;

                var totalInKindDonations = await _context.InKindDonations
                    .CountAsync(ikd => ikd.UserID == userId);

                var monthlyRecurring = 0.0; // Implement recurring donation tracking

                return new DonationStatistics
                {
                    TotalMonetaryDonated = totalMonetaryDonated,
                    TotalMonetaryDonations = totalMonetaryDonations,
                    TotalInKindDonations = totalInKindDonations,
                    MonthlyRecurringAmount = monthlyRecurring
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting donation statistics for user {userId}");
                return new DonationStatistics();
            }
        }

        // ADMIN: Get All Monetary Donations
        public async Task<List<MonetaryDonation>> GetAllMonetaryDonationsAsync()
        {
            return await _context.MonetaryDonations
                .Include(md => md.MonetaryDonationStatus)
                .Include(md => md.User)
                .OrderByDescending(md => md.Date)
                .ToListAsync();
        }

        // ADMIN: Get All In-Kind Donations
        public async Task<List<InKindDonation>> GetAllInKindDonationsAsync()
        {
            return await _context.InKindDonations
                .Include(ikd => ikd.InKindStatus)
                .Include(ikd => ikd.User)
                .OrderByDescending(ikd => ikd.DeliveryDate)
                .ToListAsync();
        }

        // ADMIN: Get Pending In-Kind Donations
        public async Task<List<InKindDonation>> GetPendingInKindDonationsAsync()
        {
            return await _context.InKindDonations
                .Include(ikd => ikd.InKindStatus)
                .Include(ikd => ikd.User)
                .Where(ikd => ikd.InKindStatus.Status == "Pending")
                .OrderBy(ikd => ikd.DeliveryDate)
                .ToListAsync();
        }

        // ADMIN: Update Monetary Donation Status
        public async Task<bool> UpdateMonetaryDonationStatusAsync(string donationId, string statusId)
        {
            try
            {
                var donation = await _context.MonetaryDonations
                    .FindAsync(donationId);

                if (donation == null)
                {
                    _logger.LogWarning($"Monetary donation not found: {donationId}");
                    return false;
                }

                donation.MonetaryDonationStatusID = statusId;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated monetary donation {donationId} status to {statusId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating monetary donation status for {donationId}");
                return false;
            }
        }

        // ADMIN: Update In-Kind Donation Status
        public async Task<bool> UpdateInKindDonationStatusAsync(string donationId, string statusId)
        {
            try
            {
                var donation = await _context.InKindDonations
                    .FindAsync(donationId);

                if (donation == null)
                {
                    _logger.LogWarning($"In-kind donation not found: {donationId}");
                    return false;
                }

                donation.InKindStatusID = statusId;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated in-kind donation {donationId} status to {statusId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating in-kind donation status for {donationId}");
                return false;
            }
        }

        // Get Monetary Donation By ID
        public async Task<MonetaryDonation> GetMonetaryDonationByIdAsync(string donationId)
        {
            return await _context.MonetaryDonations
                .Include(md => md.MonetaryDonationStatus)
                .Include(md => md.User)
                .FirstOrDefaultAsync(md => md.MonetaryDonationID == donationId);
        }

        // Get In-Kind Donation By ID
        public async Task<InKindDonation> GetInKindDonationByIdAsync(string donationId)
        {
            return await _context.InKindDonations
                .Include(ikd => ikd.InKindStatus)
                .Include(ikd => ikd.User)
                .FirstOrDefaultAsync(ikd => ikd.InKindDonationID == donationId);
        }

        // Get All Statuses
        public async Task<List<MonetaryDonationStatus>> GetMonetaryDonationStatusesAsync()
        {
            return await _context.MonetaryDonationStatuses.ToListAsync();
        }

        public async Task<List<InKindStatus>> GetInKindStatusesAsync()
        {
            return await _context.InKindStatuses.ToListAsync();
        }

        // Get Status By Name
        public async Task<MonetaryDonationStatus> GetMonetaryDonationStatusByNameAsync(string statusName)
        {
            return await _context.MonetaryDonationStatuses
                .FirstOrDefaultAsync(s => s.Status == statusName);
        }

        public async Task<InKindStatus> GetInKindStatusByNameAsync(string statusName)
        {
            return await _context.InKindStatuses
                .FirstOrDefaultAsync(s => s.Status == statusName);
        }
    }

    // Helper Class: Donation Statistics
    public class DonationStatistics
    {
        public double TotalMonetaryDonated { get; set; }
        public int TotalMonetaryDonations { get; set; }
        public int TotalInKindDonations { get; set; }
        public double MonthlyRecurringAmount { get; set; }
    }
}