namespace Calendar.Dto.Appointments;

using Calendar.Dto.Events;

public class AppointmentDto
{
    public int Id { get; set; }
    public string Type { get; set; } = ""; // "Task" or "Event"
    public string Title { get; set; } = "";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Description { get; set; }

    public string OrganizerName { get; set; } = "Unknown";  // ✅ Added
    public List<ParticipantDto>? Participants { get; set; }
}