namespace Calendar.Dto.Users;

public class UserUpdateDto
{
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Password { get; set; }
}