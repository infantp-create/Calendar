namespace Calendar.Dto.Events;

public class EventResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int CreatedByUserId { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
}