using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions.Services
{
    public class EventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get User's Volunteer Events
        public async Task<List<UserEvent>> GetUserEventsAsync(string userId)
        {
            return await _context.UserEvents
                .Include(ue => ue.Event)
                .Include(ue => ue.AttendanceStatus)
                .Where(ue => ue.UserID == userId)
                .OrderByDescending(ue => ue.Event.Date)
                .ToListAsync();
        }
    }
}