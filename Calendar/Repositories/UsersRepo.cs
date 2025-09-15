using Calendar.Data;
using Calendar.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Repositories;

public class UsersRepo : IUsersRepo
{
    private readonly AppDbContext _context;

    public UsersRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Users>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<Users?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<Users?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUser(Users user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(Users user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(Users user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExists(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}