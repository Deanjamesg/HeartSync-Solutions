using HeartSyncSolutions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Services
{
    public interface IEventService
    {
        // Event CRUD Operations
        Task<Event> CreateEventAsync(Event eventItem);
        Task<Event> GetEventByIdAsync(string eventId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<bool> UpdateEventAsync(Event eventItem);
        Task<bool> DeleteEventAsync(string eventId);

        // Event Filtering
        Task<IEnumerable<Event>> GetEventsByStatusAsync(string statusId);
        Task<IEnumerable<Event>> GetEventsByTypeAsync(string typeId);
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetPastEventsAsync();
        Task<IEnumerable<Event>> GetEventsByLocationAsync(string location);

        // Volunteer Management
        Task<bool> RegisterVolunteerForEventAsync(string eventId, string userId, string attendanceStatusId);
        Task<bool> UnregisterVolunteerFromEventAsync(string eventId, string userId);
        Task<bool> UpdateVolunteerAttendanceStatusAsync(string userEventId, string newAttendanceStatusId);
        Task<IEnumerable<UserEvent>> GetVolunteersForEventAsync(string eventId);
        Task<IEnumerable<UserEvent>> GetVolunteersByAttendanceStatusAsync(string eventId, string attendanceStatusId);
        Task<int> GetVolunteerCountForEventAsync(string eventId);

        // Event Gallery Management
        Task<bool> AddEventGalleryImageAsync(EventGallery galleryImage);
        Task<IEnumerable<EventGallery>> GetEventGalleryImagesAsync(string eventId);
        Task<bool> DeleteEventGalleryImageAsync(string galleryImageId);

        // Event Report Management
        Task<EventReport> CreateEventReportAsync(EventReport report);
        Task<EventReport> GetEventReportByEventIdAsync(string eventId);
        Task<bool> UpdateEventReportAsync(EventReport report);

        // Statistics
        Task<int> GetTotalEventCountAsync();
        Task<Dictionary<string, int>> GetEventCountByStatusAsync();
        Task<Dictionary<string, int>> GetEventCountByTypeAsync();
    }
}