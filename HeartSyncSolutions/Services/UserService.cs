using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> GetVolunteersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> GetDonorsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(bool isVolunteer, bool isDonor)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<Event>> GetUserEventsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserEvent>> GetUserEventRegistrationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Event>> GetUserUpcomingEventsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Event>> GetUserPastEventsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserRegisteredForEventAsync(string userId, int eventId)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<MonetaryDonations>> GetUserMonetaryDonationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<double> GetUserTotalDonationAmountAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InKindOffer>> GetUserInKindOffersAsync(string userId)
        {
            throw new NotImplementedException();
        }


        public async Task<PartnerPledge> GetUserPartnerPledgeAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasActivePartnerPledgeAsync(string userId)
        {
            throw new NotImplementedException();
        }


        public async Task<int> GetTotalUserCountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetVolunteerCountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetDonorCountAsync()
        {

            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, int>> GetUserCountByRoleAsync()
        {
            throw new NotImplementedException();
        }

    }
}