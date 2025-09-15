using System.ComponentModel.DataAnnotations;

namespace Calendar.Models;

public class Users
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    [Required]
    public string Email { get; set; } = "";
    [Required]
    public string PasswordHash { get; set; } = "";

    // Navigation
    public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    public ICollection<Events> CreatedEvents { get; set; } = new List<Events>();
    public ICollection<EventParticipants> EventParticipants { get; set; } = new List<EventParticipants>();
    
}