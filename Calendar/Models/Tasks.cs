using System.ComponentModel.DataAnnotations;

namespace Calendar.Models;

public class Tasks
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [Required] public DateTime Start { get; set; }   // Changed to DateTime
    [Required] public DateTime End { get; set; }     // Changed to DateTime

    public string? Description { get; set; }
    
    // In Tasks and Events models
    public string? RecurrenceType { get; set; }   // "Daily", "Weekly", "Monthly", "Custom", or null
    public int? RecurrenceCount { get; set; }    // Number of occurrences
    public List<string>? RecurrenceDays { get; set; }  // For weekly: "Mon,Wed,Fri" etc.
    public DateTime? RecurrenceEnd { get; set; } // Optional end date


    // Navigation
    public Users? User { get; set; }
}