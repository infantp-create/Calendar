using Calendar.Models;

namespace Calendar.Repositories;


public interface IEventsRepo
{
    Task<List<Events>> GetEventsByUser(int userId);
    Task<Events> AddEvent(Events ev);
    Task<Events?> UpdateEvent(Events ev);
    Task<bool> DeleteEvent(int eventId);
    Task<bool> RemoveParticipant(int eventId, int userId);
    Task<bool> CheckEventConflict(int userId, DateTime start, DateTime end, int? excludeId = null);
}