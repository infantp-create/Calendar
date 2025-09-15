using Calendar.Repositories;
using Calendar.Models;
using Calendar.Dto.Users;

namespace Calendar.Services;

public class UsersService
{
    private readonly IUsersRepo _usersRepo;

    public UsersService(IUsersRepo usersRepo)
    {
        _usersRepo = usersRepo;
    }

    public async Task<List<UserDto>> GetUsers()
    {
        var users = await _usersRepo.GetUsers();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.Name,
            Email = u.Email
        }).ToList();
    }

    public async Task<(bool Success, string? Error, UserDto? User)> Register(UserRegisterDto dto)
    {
        if (await _usersRepo.EmailExists(dto.Email))
            return (false, "Email already registered.", null);

        var user = new Users
        {
            Name = dto.UserName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _usersRepo.AddUser(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.Name,
            Email = user.Email
        };

        return (true, null, userDto);
    }

    public async Task<(bool Success, string? Error,string? Token, UserDto? User)> Login(UserLoginDto dto, JwtService jwtService)
    {
        var user = await _usersRepo.GetUserByEmail(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (false, "Invalid email or password.", null);

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.Name,
            Email = user.Email
        };
        
        var token = jwtService.GenerateToken(user.Id, user.Name, user.Email);

        return (true, null, token, userDto);
    }

    public async Task<(bool Success, string? Error, UserDto? User)> UpdateUser(int id, UserUpdateDto dto)
    {
        var user = await _usersRepo.GetUserById(id);
        if (user == null) return (false, "User not found.", null);

        user.Name = dto.UserName;
        user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _usersRepo.UpdateUser(user);

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
        var user = await _usersRepo.GetUserById(id);
        if (user == null) return (false, "User not found.");

        await _usersRepo.DeleteUser(user);
        return (true, null);
    }
}
