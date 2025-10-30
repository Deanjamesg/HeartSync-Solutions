using HeartSyncSolutions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Services
{
    public interface IUserService
    {
        // User CRUD Operations
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(string userId);

        // User Filtering
        Task<IEnumerable<ApplicationUser>> GetVolunteersAsync();
        Task<IEnumerable<ApplicationUser>> GetDonorsAsync();
        Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(bool isVolunteer, bool isDonor);

        // User Event Management
        Task<IEnumerable<Event>> GetUserEventsAsync(string userId);
        Task<IEnumerable<UserEvent>> GetUserEventRegistrationsAsync(string userId);
        Task<IEnumerable<Event>> GetUserUpcomingEventsAsync(string userId);
        Task<IEnumerable<Event>> GetUserPastEventsAsync(string userId);
        Task<bool> IsUserRegisteredForEventAsync(string userId, int eventId);

        // User Donation Management
        Task<IEnumerable<MonetaryDonations>> GetUserMonetaryDonationsAsync(string userId);
        Task<double> GetUserTotalDonationAmountAsync(string userId);
        Task<IEnumerable<InKindOffer>> GetUserInKindOffersAsync(string userId);

        // User Partner Pledge Management
        Task<PartnerPledge> GetUserPartnerPledgeAsync(string userId);
        Task<bool> HasActivePartnerPledgeAsync(string userId);

        // Statistics
        Task<int> GetTotalUserCountAsync();
        Task<int> GetVolunteerCountAsync();
        Task<int> GetDonorCountAsync();
        Task<Dictionary<string, int>> GetUserCountByRoleAsync();
    }
}