using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateEventAsync(Event eventItem)
        {
            if (eventItem == null)
                throw new ArgumentNullException(nameof(eventItem));

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task<Event> GetEventByIdAsync(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return null;

            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Include(e => e.UserEvents)
                    .ThenInclude(ue => ue.ApplicationUser)
                .Include(e => e.UserEvents)
                    .ThenInclude(ue => ue.AttendanceStatus)
                .Include(e => e.GalleryImages)
                .Include(e => e.EventReport)
                .FirstOrDefaultAsync(e => e.EventID == eventId);
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Include(e => e.UserEvents)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateEventAsync(Event eventItem)
        {
            if (eventItem == null)
                throw new ArgumentNullException(nameof(eventItem));

            var existingEvent = await _context.Events.FindAsync(eventItem.EventID);
            if (existingEvent == null)
                return false;

            existingEvent.Name = eventItem.Name;
            existingEvent.Location = eventItem.Location;
            existingEvent.Date = eventItem.Date;
            existingEvent.EventTypeID = eventItem.EventTypeID;
            existingEvent.EventStatusID = eventItem.EventStatusID;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventAsync(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return false;

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
                return false;

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<Event>> GetEventsByStatusAsync(string statusId)
        {
            if (string.IsNullOrWhiteSpace(statusId))
                return new List<Event>();

            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Where(e => e.EventStatusID == statusId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByTypeAsync(string typeId)
        {
            if (string.IsNullOrWhiteSpace(typeId))
                return new List<Event>();

            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Where(e => e.EventTypeID == typeId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            var today = DateTime.Today;
            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Where(e => e.Date >= today)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPastEventsAsync()
        {
            var today = DateTime.Today;
            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Where(e => e.Date < today)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return new List<Event>();

            return await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.EventStatus)
                .Where(e => e.Location.Contains(location))
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<bool> RegisterVolunteerForEventAsync(string eventId, string userId, string attendanceStatusId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(eventId) || string.IsNullOrWhiteSpace(attendanceStatusId))
                return false;

            // Check if already registered
            var existingRegistration = await _context.UserEvents
                .FirstOrDefaultAsync(ue => ue.EventID == eventId && ue.ApplicationUserID == userId);

            if (existingRegistration != null)
                return false; // Already registered

            var userEvent = new UserEvent
            {
                EventID = eventId,
                ApplicationUserID = userId,
                AttendanceStatusID = attendanceStatusId
            };

            _context.UserEvents.Add(userEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnregisterVolunteerFromEventAsync(string eventId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(eventId))
                return false;

            var userEvent = await _context.UserEvents
                .FirstOrDefaultAsync(ue => ue.EventID == eventId && ue.ApplicationUserID == userId);

            if (userEvent == null)
                return false;

            _context.UserEvents.Remove(userEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVolunteerAttendanceStatusAsync(string userEventId, string newAttendanceStatusId)
        {
            if (string.IsNullOrWhiteSpace(userEventId) || string.IsNullOrWhiteSpace(newAttendanceStatusId))
                return false;

            var userEvent = await _context.UserEvents.FindAsync(userEventId);
            if (userEvent == null)
                return false;

            userEvent.AttendanceStatusID = newAttendanceStatusId;
            _context.UserEvents.Update(userEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserEvent>> GetVolunteersForEventAsync(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return new List<UserEvent>();

            return await _context.UserEvents
                .Include(ue => ue.ApplicationUser)
                .Include(ue => ue.AttendanceStatus)
                .Where(ue => ue.EventID == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserEvent>> GetVolunteersByAttendanceStatusAsync(string eventId, string attendanceStatusId)
        {
            if (string.IsNullOrWhiteSpace(eventId) || string.IsNullOrWhiteSpace(attendanceStatusId))
                return new List<UserEvent>();

            return await _context.UserEvents
                .Include(ue => ue.ApplicationUser)
                .Include(ue => ue.AttendanceStatus)
                .Where(ue => ue.EventID == eventId && ue.AttendanceStatusID == attendanceStatusId)
                .ToListAsync();
        }

        public async Task<int> GetVolunteerCountForEventAsync(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return 0;

            return await _context.UserEvents
                .Where(ue => ue.EventID == eventId)
                .CountAsync();
        }


        public async Task<bool> AddEventGalleryImageAsync(EventGallery galleryImage)
        {
            if (galleryImage == null)
                throw new ArgumentNullException(nameof(galleryImage));

            _context.EventGalleries.Add(galleryImage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventGallery>> GetEventGalleryImagesAsync(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return new List<EventGallery>();

            return await _context.EventGalleries
                .Where(eg => eg.EventID == eventId)
                .ToListAsync();
        }

        public async Task<bool> DeleteEventGalleryImageAsync(string galleryImageId)
        {
            if (string.IsNullOrWhiteSpace(galleryImageId))
                return false;

            var galleryImage = await _context.EventGalleries.FindAsync(galleryImageId);
            if (galleryImage == null)
                return false;

            _context.EventGalleries.Remove(galleryImage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EventReport> CreateEventReportAsync(EventReport report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            // Check if report already exists for this event
            var existingReport = await _context.EventReports
                .FirstOrDefaultAsync(er => er.EventID == report.EventID);

            if (existingReport != null)
                throw new InvalidOperationException("A report already exists for this event.");

            _context.EventReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<EventReport> GetEventReportByEventIdAsync(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return null;

            return await _context.EventReports
                .Include(er => er.Event)
                .FirstOrDefaultAsync(er => er.EventID == eventId);
        }

        public async Task<bool> UpdateEventReportAsync(EventReport report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            var existingReport = await _context.EventReports.FindAsync(report.EventID);
            if (existingReport == null)
                return false;

            existingReport.Summary = report.Summary;

            _context.EventReports.Update(existingReport);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalEventCountAsync()
        {
            return await _context.Events.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetEventCountByStatusAsync()
        {
            return await _context.Events
                .Include(e => e.EventStatus)
                .GroupBy(e => e.EventStatus.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetEventCountByTypeAsync()
        {
            return await _context.Events
                .Include(e => e.EventType)
                .GroupBy(e => e.EventType.Title)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }
    }
}