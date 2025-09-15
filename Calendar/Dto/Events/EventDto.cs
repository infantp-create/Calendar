namespace Calendar.Dto.Events;
public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Description { get; set; } = "";

    public int CreatedByUserId { get; set; }
    public string OrganizerName { get; set; } = "";   // new field

    public List<int> ParticipantIds { get; set; } = new();
    public List<string> ParticipantNames { get; set; } = new(); // new field

    public string? RecurrenceType { get; set; }
    public int? RecurrenceCount { get; set; }
    public List<string>? RecurrenceDays { get; set; }
    public DateTime? RecurrenceEnd { get; set; }
}
