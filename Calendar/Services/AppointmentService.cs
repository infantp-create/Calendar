using Calendar.Dto.Appointments;
using Calendar.Repositories;
using Calendar.Dto.Events;
using Microsoft.EntityFrameworkCore;
using Calendar.GetRecurrence;

namespace Calendar.Services;

public class AppointmentsService
{
    private readonly ITasksRepo _tasksRepo;
    private readonly IEventsRepo _eventsRepo;
    private readonly IRecurrence _recurrence;

    public AppointmentsService(ITasksRepo tasksRepo, IEventsRepo eventsRepo)
    {
        _tasksRepo = tasksRepo;
        _eventsRepo = eventsRepo;
        _recurrence = new Recurrence();
    }

    // Get appointments for a user on a specific day or range
    public async Task<List<AppointmentDto>> GetAppointmentsForUser(int userId, DateTime startDate, DateTime? endDate = null, string? type = null)
    {
        endDate ??= startDate.Date.AddDays(1).AddTicks(-1); // Default: full day

        var appointments = new List<AppointmentDto>();

        // -------------------- TASKS --------------------
        if (type == null || type == "Task")
        {
            var tasks = await _tasksRepo.GetTasksByUser(userId);

            foreach (var task in tasks)
            {
                var occurrences = _recurrence.GenerateOccurrences(task.Start, task.End, task.RecurrenceType, task.RecurrenceCount, task.RecurrenceDays, task.RecurrenceEnd);

                foreach (var occ in occurrences)
                {
                    if (occ.End > startDate && occ.Start < endDate)
                    {
                        appointments.Add(new AppointmentDto
                        {
                            Id = task.Id,
                            Type = "Task",
                            Title = task.Title,
                            Start = occ.Start,
                            End = occ.End,
                            Description = task.Description,
                            OrganizerName = "Self", // ✅ Task owned by the user
                            Participants = null     // ✅ Tasks don’t have participants
                        });
                    }
                }
            }
        }

        // -------------------- EVENTS --------------------
        if (type == null || type == "Event")
        {
            var events = await _eventsRepo.GetEventsByUser(userId);

            foreach (var ev in events)
            {
                // Include only if the user is a participant or organizer
                if (!ev.Participants.Any(p => p.UserId == userId) && ev.CreatedByUserId != userId) 
                    continue;

                var occurrences = _recurrence.GenerateOccurrences(ev.Start, ev.End, ev.RecurrenceType, ev.RecurrenceCount, ev.RecurrenceDays, ev.RecurrenceEnd);

                foreach (var occ in occurrences)
                {
                    if (occ.End > startDate && occ.Start < endDate)
                    {
                        appointments.Add(new AppointmentDto
                        {
                            Id = ev.Id,
                            Type = "Event",
                            Title = ev.Title,
                            Start = occ.Start,
                            End = occ.End,
                            Description = ev.Description,
                            OrganizerName = ev.CreatedByUser?.Name ?? "Unknown", // ✅ Organizer name
                            Participants = ev.Participants.Select(p => new ParticipantDto
                            {
                                UserId = p.UserId,
                                UserName = p.User!.Name,
                                Email = p.User.Email
                            }).ToList()
                        });
                    }
                }
            }
        }

        // Sort by start time
        return appointments.OrderBy(a => a.Start).ToList();
    }
}

