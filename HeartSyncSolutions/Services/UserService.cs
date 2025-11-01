using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<UserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Get User By ID
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Get User By Email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // Check if Email Exists
        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        // Update User Profile
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {user.Id}");
                return false;
            }
        }

        // Get User Statistics
        public async Task<UserStatistics> GetUserStatisticsAsync(string userId)
        {
            var totalEvents = await _context.UserEvents
                .CountAsync(ue => ue.UserID == userId);

            var attendedEvents = await _context.UserEvents
                .Include(ue => ue.AttendanceStatus)
                .CountAsync(ue => ue.UserID == userId && ue.AttendanceStatus.Status == "Attended");

            var totalMonetaryDonations = await _context.MonetaryDonations
                .Where(md => md.UserID == userId)
                .SumAsync(md => md.DonationAmount);

            var totalInKindDonations = await _context.InKindDonations
                .CountAsync(ikd => ikd.UserID == userId);

            return new UserStatistics
            {
                TotalEventsSignedUp = totalEvents,
                TotalEventsAttended = attendedEvents,
                TotalMonetaryDonated = totalMonetaryDonations,
                TotalInKindDonations = totalInKindDonations
            };
        }

        // Add User to Role
        public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.AddToRoleAsync(user, roleName);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding user {userId} to role {roleName}");
                return false;
            }
        }

    }

    // Helper Class: User Statistics
    public class UserStatistics
    {
        public int TotalEventsSignedUp { get; set; }
        public int TotalEventsAttended { get; set; }
        public double TotalMonetaryDonated { get; set; }
        public int TotalInKindDonations { get; set; }
    }
}