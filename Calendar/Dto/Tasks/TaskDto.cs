namespace Calendar.Dto.Tasks;

public class TaskDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = "";
    public DateTime Start { get; set; }   // now DateTime
    public DateTime End { get; set; }     // now DateTime
    public string? Description { get; set; }
    
    // In Tasks and Events models
    public string? RecurrenceType { get; set; }   // "Daily", "Weekly", "Monthly", "Custom", or null
    public int? RecurrenceCount { get; set; }    // Number of occurrences
    public List<string>? RecurrenceDays { get; set; }  // For weekly: "Mon,Wed,Fri" etc.
    public DateTime? RecurrenceEnd { get; set; } // Optional end date

}