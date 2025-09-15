using Calendar.Data;
using Calendar.Models;
using Calendar.Dto.Users;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Services;

public class UsersService
{
    private readonly AppDbContext _context;

    public UsersService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetUsers()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.Name,
                Email = u.Email
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error, UserDto? User)> Register(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return (false, "Email already registered.", null);

        var user = new Users
        {
            Name = dto.UserName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.Name,
            Email = user.Email
        };

        return (true, null, userDto);
    }

    public async Task<(bool Success, string? Error, UserDto? User)> Login(UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (false, "Invalid email or password.", null);

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.Name,
            Email = user.Email
        };

        return (true, null, userDto);
    }

    public async Task<(bool Success, string? Error, UserDto? User)> UpdateUser(int id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return (false, "User not found.", null);

        user.Name = dto.UserName;
        user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.Name,
            Email = user.Email
        };

        return (true, null, userDto);
    }

    public async Task<(bool Success, string? Error)> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return (false, "User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
