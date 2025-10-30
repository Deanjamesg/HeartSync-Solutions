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
        Task<Event> GetEventByIdAsync(int eventId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<bool> UpdateEventAsync(Event eventItem);
        Task<bool> DeleteEventAsync(int eventId);

        // Event Filtering
        Task<IEnumerable<Event>> GetEventsByStatusAsync(int statusId);
        Task<IEnumerable<Event>> GetEventsByTypeAsync(int typeId);
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetPastEventsAsync();
        Task<IEnumerable<Event>> GetEventsByLocationAsync(string location);

        // Volunteer Management
        Task<bool> RegisterVolunteerForEventAsync(int eventId, string userId, int attendanceStatusId);
        Task<bool> UnregisterVolunteerFromEventAsync(int eventId, string userId);
        Task<bool> UpdateVolunteerAttendanceStatusAsync(int userEventId, int newAttendanceStatusId);
        Task<IEnumerable<UserEvent>> GetVolunteersForEventAsync(int eventId);
        Task<IEnumerable<UserEvent>> GetVolunteersByAttendanceStatusAsync(int eventId, int attendanceStatusId);
        Task<int> GetVolunteerCountForEventAsync(int eventId);

        // Event Gallery Management
        Task<bool> AddEventGalleryImageAsync(EventGallery galleryImage);
        Task<IEnumerable<EventGallery>> GetEventGalleryImagesAsync(int eventId);
        Task<bool> DeleteEventGalleryImageAsync(int galleryImageId);

        // Event Report Management
        Task<EventReport> CreateEventReportAsync(EventReport report);
        Task<EventReport> GetEventReportByEventIdAsync(int eventId);
        Task<bool> UpdateEventReportAsync(EventReport report);

        // Statistics
        Task<int> GetTotalEventCountAsync();
        Task<Dictionary<string, int>> GetEventCountByStatusAsync();
        Task<Dictionary<string, int>> GetEventCountByTypeAsync();
    }
}