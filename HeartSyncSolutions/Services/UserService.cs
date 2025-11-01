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
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _context.Users
                .Include(u => u.UserEvents)
                    .ThenInclude(ue => ue.Event)
                .Include(u => u.MonetaryDonations)
                .Include(u => u.InKindOffers)
                .Include(u => u.PartnerPledge)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Users
                .Include(u => u.UserEvents)
                .Include(u => u.MonetaryDonations)
                .Include(u => u.InKindOffers)
                .Include(u => u.PartnerPledge)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserEvents)
                .Include(u => u.MonetaryDonations)
                .Include(u => u.InKindOffers)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                return false;

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.ContactNumber = user.ContactNumber;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.IsVolunteer = user.IsVolunteer;
            existingUser.IsDonor = user.IsDonor;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ApplicationUser>> GetVolunteersAsync()
        {
            return await _context.Users
                .Where(u => u.IsVolunteer == true)
                .Include(u => u.UserEvents)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetDonorsAsync()
        {
            return await _context.Users
                .Where(u => u.IsDonor == true)
                .Include(u => u.MonetaryDonations)
                .Include(u => u.InKindOffers)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(bool isVolunteer, bool isDonor)
        {
            return await _context.Users
                .Where(u => u.IsVolunteer == isVolunteer && u.IsDonor == isDonor)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUserEventsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Event>();

            return await _context.UserEvents
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventType)
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventStatus)
                .Include(ue => ue.AttendanceStatus)
                .Where(ue => ue.ApplicationUserID == userId)
                .Select(ue => ue.Event)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserEvent>> GetUserEventRegistrationsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<UserEvent>();

            return await _context.UserEvents
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventType)
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventStatus)
                .Include(ue => ue.AttendanceStatus)
                .Where(ue => ue.ApplicationUserID == userId)
                .OrderByDescending(ue => ue.Event.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUserUpcomingEventsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Event>();

            var today = DateTime.Today;
            return await _context.UserEvents
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventType)
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventStatus)
                .Where(ue => ue.ApplicationUserID == userId && ue.Event.Date >= today)
                .Select(ue => ue.Event)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUserPastEventsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Event>();

            var today = DateTime.Today;
            return await _context.UserEvents
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventType)
                .Include(ue => ue.Event)
                    .ThenInclude(e => e.EventStatus)
                .Where(ue => ue.ApplicationUserID == userId && ue.Event.Date < today)
                .Select(ue => ue.Event)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<bool> IsUserRegisteredForEventAsync(string userId, string eventId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(eventId))
                return false;

            return await _context.UserEvents
                .AnyAsync(ue => ue.ApplicationUserID == userId && ue.EventID == eventId);
        }

        public async Task<IEnumerable<MonetaryDonations>> GetUserMonetaryDonationsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<MonetaryDonations>();

            return await _context.MonetaryDonations
                .Where(d => d.ApplicationUserID == userId)
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }

        public async Task<double> GetUserTotalDonationAmountAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return 0;

            var total = await _context.MonetaryDonations
                .Where(d => d.ApplicationUserID == userId)
                .SumAsync(d => d.DonationAmount);
            return total;
        }

        public async Task<IEnumerable<InKindOffer>> GetUserInKindOffersAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<InKindOffer>();

            return await _context.InKindOffers
                .Where(o => o.ApplicationUserID == userId)
                .OrderByDescending(o => o.OfferedDate)
                .ToListAsync();
        }

        public async Task<PartnerPledge> GetUserPartnerPledgeAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _context.PartnerPledges
                .FirstOrDefaultAsync(pp => pp.ApplicationUserID == userId);
        }

        public async Task<bool> HasActivePartnerPledgeAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            return await _context.PartnerPledges
                .AnyAsync(pp => pp.ApplicationUserID == userId && pp.Status == "Active");
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetVolunteerCountAsync()
        {
            return await _context.Users
                .Where(u => u.IsVolunteer == true)
                .CountAsync();
        }

        public async Task<int> GetDonorCountAsync()
        {
            return await _context.Users
                .Where(u => u.IsDonor == true)
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetUserCountByRoleAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var volunteerCount = await _context.Users.Where(u => u.IsVolunteer == true).CountAsync();
            var donorCount = await _context.Users.Where(u => u.IsDonor == true).CountAsync();
            var bothCount = await _context.Users.Where(u => u.IsVolunteer == true && u.IsDonor == true).CountAsync();

            return new Dictionary<string, int>
            {
                { "Total", totalUsers },
                { "Volunteers", volunteerCount },
                { "Donors", donorCount },
                { "Both", bothCount }
            };
        }
    }
}