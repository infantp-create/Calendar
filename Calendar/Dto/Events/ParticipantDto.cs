namespace Calendar.Dto.Events;

public class ParticipantDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
}