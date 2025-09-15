namespace Calendar.Models;

public class EventParticipants
{
    public int EventId { get; set; }
    public int UserId { get; set; }

    // Navigation
    public Events? Event { get; set; }
    public Users? User { get; set; }
}

