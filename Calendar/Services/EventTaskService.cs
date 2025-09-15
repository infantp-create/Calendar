using Calendar.Dto.Events;
using Calendar.Dto.Tasks;
using Calendar.Models;
using Calendar.GetRecurrence;
using Calendar.Repositories;

namespace Calendar.Services;

public class EventTaskService
{
    private readonly ITasksRepo _tasksRepo;
    private readonly IEventsRepo _eventsRepo;
    private readonly IRecurrence _recurrence;

    public EventTaskService(ITasksRepo tasksRepo, IEventsRepo eventsRepo, IRecurrence recurrence)
    {
        _tasksRepo = tasksRepo;
        _eventsRepo = eventsRepo;
        _recurrence = recurrence;
    }

    // -------------------- Tasks --------------------
    public async Task<List<TaskDto>> GetTasksForUser(int userId)
    {
        var tasks = await _tasksRepo.GetTasksByUser(userId);
        return tasks.Select(ToTaskDto).ToList();
    }

    public async Task<(bool Success, string? Error, List<TaskDto>? Tasks)> CreateTask(TaskDto dto)
    {
        if (dto.End <= dto.Start) return (false, "End must be after start.", null);

        var newOccurrences = _recurrence.GenerateOccurrences(dto.Start, dto.End,
            dto.RecurrenceType, dto.RecurrenceCount, dto.RecurrenceDays, dto.RecurrenceEnd);

        var conflict = await HasConflict(dto.UserId, newOccurrences, ignoreTaskId: null, ignoreEventId: null);
        if (conflict.Conflict) return (false, conflict.Error, null);

        var createdTasks = new List<TaskDto>();
        foreach (var occ in newOccurrences)
        {
            var task = new Tasks
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Start = occ.Start,
                End = occ.End,
                Description = dto.Description,
                RecurrenceType = dto.RecurrenceType,
                RecurrenceCount = dto.RecurrenceCount,
                RecurrenceDays = dto.RecurrenceDays,
                RecurrenceEnd = dto.RecurrenceEnd
            };

            await _tasksRepo.AddTask(task);
            createdTasks.Add(ToTaskDto(task));
        }

        return (true, null, createdTasks);
    }

    public async Task<(bool Success, string? Error, TaskDto? Task)> UpdateTask(int id, TaskDto dto)
    {
        var task = (await _tasksRepo.GetTasksByUser(dto.UserId)).FirstOrDefault(t => t.Id == id);
        if (task == null) return (false, "Task not found.", null);

        var newOccurrences = _recurrence.GenerateOccurrences(dto.Start, dto.End,
            dto.RecurrenceType, dto.RecurrenceCount, dto.RecurrenceDays, dto.RecurrenceEnd);

        var conflict = await HasConflict(dto.UserId, newOccurrences, ignoreTaskId: id);
        if (conflict.Conflict) return (false, conflict.Error, null);

        task.Title = dto.Title;
        task.Start = dto.Start;
        task.End = dto.End;
        task.Description = dto.Description;
        task.RecurrenceType = dto.RecurrenceType;
        task.RecurrenceCount = dto.RecurrenceCount;
        task.RecurrenceDays = dto.RecurrenceDays;
        task.RecurrenceEnd = dto.RecurrenceEnd;

        await _tasksRepo.UpdateTask(task);
        return (true, null, ToTaskDto(task));
    }

    public async Task<(bool Success, string? Error)> DeleteTask(int id)
    {
        bool deleted = await _tasksRepo.DeleteTask(id);
        return deleted ? (true, null) : (false, "Task not found.");
    }

    // -------------------- Events --------------------
    public async Task<List<EventDto>> GetEventsForUser(int userId)
    {
        var events = await _eventsRepo.GetEventsByUser(userId);
        return events.Select(ToEventDto).ToList();
    }

    public async Task<(bool Success, string? Error, List<EventDto>? Events)> CreateEvent(EventDto dto)
    {
        if (dto.End <= dto.Start) return (false, "End must be after start.", null);

        var newOccurrences = _recurrence.GenerateOccurrences(dto.Start, dto.End,
            dto.RecurrenceType, dto.RecurrenceCount, dto.RecurrenceDays, dto.RecurrenceEnd);

        // check conflicts for organizer and participants
        var allUsers = dto.ParticipantIds.Distinct().Append(dto.CreatedByUserId).ToList();
        foreach (var uid in allUsers)
        {
            var conflict = await HasConflict(uid, newOccurrences);
            if (conflict.Conflict) return (false, conflict.Error, null);
        }

        var createdEvents = new List<EventDto>();
        foreach (var occ in newOccurrences)
        {
            var ev = new Events
            {
                Title = dto.Title,
                Start = occ.Start,
                End = occ.End,
                Description = dto.Description,
                CreatedByUserId = dto.CreatedByUserId,
                RecurrenceType = dto.RecurrenceType,
                RecurrenceCount = dto.RecurrenceCount,
                RecurrenceDays = dto.RecurrenceDays,
                RecurrenceEnd = dto.RecurrenceEnd,
                Participants = dto.ParticipantIds
                    .Distinct()
                    .Select(u => new EventParticipants { UserId = u })
                    .ToList()
            };

            var savedEvent = await _eventsRepo.AddEvent(ev);
            createdEvents.Add(ToEventDto(savedEvent));
        }

        return (true, null, createdEvents);
    }

    public async Task<(bool Success, string? Error, EventDto? Event)> UpdateEvent(int id, EventDto dto)
    {
        var ev = (await _eventsRepo.GetEventsByUser(dto.CreatedByUserId)).FirstOrDefault(e => e.Id == id);
        if (ev == null) return (false, "Event not found.", null);

        if (dto.CreatedByUserId != ev.CreatedByUserId) return (false, "Only creator can update.", null);

        var newOccurrences = _recurrence.GenerateOccurrences(dto.Start, dto.End,
            dto.RecurrenceType, dto.RecurrenceCount, dto.RecurrenceDays, dto.RecurrenceEnd);

        var conflict = await HasConflict(dto.CreatedByUserId, newOccurrences, ignoreEventId: id);
        if (conflict.Conflict) return (false, conflict.Error, null);

        ev.Title = dto.Title;
        ev.Start = dto.Start;
        ev.End = dto.End;
        ev.Description = dto.Description;
        ev.RecurrenceType = dto.RecurrenceType;
        ev.RecurrenceCount = dto.RecurrenceCount;
        ev.RecurrenceDays = dto.RecurrenceDays;
        ev.RecurrenceEnd = dto.RecurrenceEnd;

        ev.Participants.Clear();
        ev.Participants = dto.ParticipantIds
            .Distinct()
            .Select(u => new EventParticipants { UserId = u })
            .ToList();

        await _eventsRepo.UpdateEvent(ev);
        return (true, null, ToEventDto(ev));
    }

    public async Task<(bool Success, string? Error)> DeleteEvent(int eventId, int userId)
    {
        var ev = (await _eventsRepo.GetEventsByUser(userId))
            .FirstOrDefault(e => e.Id == eventId);

        if (ev == null)
            return (false, "Event not found.");

        if (ev.CreatedByUserId == userId)
            await _eventsRepo.DeleteEvent(eventId); // Organizer deletes fully
        else
        {
            if (!ev.Participants.Any(p => p.UserId == userId))
                return (false, "User is not a participant.");

            await _eventsRepo.RemoveParticipant(eventId, userId); // Participant deletes self
        }

        return (true, null);
    }

    // -------------------- Helpers --------------------
    private TaskDto ToTaskDto(Tasks t) => new()
    {
        Id = t.Id,
        UserId = t.UserId,
        Title = t.Title,
        Start = t.Start,
        End = t.End,
        Description = t.Description,
        RecurrenceType = t.RecurrenceType,
        RecurrenceCount = t.RecurrenceCount,
        RecurrenceDays = t.RecurrenceDays,
        RecurrenceEnd = t.RecurrenceEnd
    };

    private EventDto ToEventDto(Events e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Start = e.Start,
        End = e.End,
        Description = e.Description,
        CreatedByUserId = e.CreatedByUserId,
        OrganizerName = e.CreatedByUser?.Name ?? "Unknown",
        ParticipantIds = e.Participants.Select(p => p.UserId).ToList(),
        RecurrenceType = e.RecurrenceType,
        RecurrenceCount = e.RecurrenceCount,
        RecurrenceDays = e.RecurrenceDays,
        RecurrenceEnd = e.RecurrenceEnd
    };

    private bool IsOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && start2 < end1;
    }

    private async Task<(bool Conflict, string? Error)> HasConflict(
        int userId,
        List<(DateTime Start, DateTime End)> newOccurrences,
        int? ignoreTaskId = null,
        int? ignoreEventId = null)
    {
        var existingTasks = await _tasksRepo.GetTasksByUser(userId);
        var existingEvents = await _eventsRepo.GetEventsByUser(userId);

        foreach (var occ in newOccurrences)
        {
            foreach (var task in existingTasks.Where(t => t.Id != ignoreTaskId))
            {
                var taskOccurrences = _recurrence.GenerateOccurrences(
                    task.Start, task.End, task.RecurrenceType,
                    task.RecurrenceCount, task.RecurrenceDays, task.RecurrenceEnd);

                if (taskOccurrences.Any(ex => IsOverlap(ex.Start, ex.End, occ.Start, occ.End)))
                    return (true, $"User {userId} has task conflict.");
            }

            foreach (var ev in existingEvents.Where(e => e.Id != ignoreEventId))
            {
                var eventOccurrences = _recurrence.GenerateOccurrences(
                    ev.Start, ev.End, ev.RecurrenceType,
                    ev.RecurrenceCount, ev.RecurrenceDays, ev.RecurrenceEnd);

                if (eventOccurrences.Any(ex => IsOverlap(ex.Start, ex.End, occ.Start, occ.End)))
                    return (true, $"User {userId} has event conflict.");
            }
        }

        return (false, null);
    }
}
