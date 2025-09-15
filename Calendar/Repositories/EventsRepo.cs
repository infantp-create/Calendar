using Calendar.Data;
using Calendar.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Repositories;

public class EventsRepo : IEventsRepo
{
    private readonly AppDbContext _context;
    public EventsRepo(AppDbContext context) => _context = context;

    public async Task<List<Events>> GetEventsByUser(int userId)
        =>await _context.Events
            .Include(e => e.CreatedByUser)
            .Include(e => e.Participants).ThenInclude(p => p.User)
            .Where(e => e.CreatedByUserId == userId || e.Participants.Any(p => p.UserId == userId))
            .ToListAsync();

    public async Task<Events> AddEvent(Events ev)
    {
        _context.Events.Add(ev);
        await _context.SaveChangesAsync();
        return ev;
    }

    public async Task<Events?> UpdateEvent(Events ev)
    {
        _context.Events.Update(ev);
        await _context.SaveChangesAsync();
        return ev;
    }

    public async Task<bool> DeleteEvent(int eventId)
    {
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return false;
        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> RemoveParticipant(int eventId, int userId)
    {
        var participant = await _context.EventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);
        if (participant == null) return false;

        _context.EventParticipants.Remove(participant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckEventConflict(int userId, DateTime start, DateTime end, int? excludeId = null)
    {
        return await _context.Events.Include(e => e.Participants)
            .AnyAsync(e => e.Participants.Any(p => p.UserId == userId) &&
                           (excludeId == null || e.Id != excludeId) &&
                           e.Start < end && e.End > start);
    }
}