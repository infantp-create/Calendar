using Calendar.Models;

namespace Calendar.Repositories;

public interface IUsersRepo
{
    Task<List<Users>> GetUsers();
    Task<Users?> GetUserById(int id);
    Task<Users?> GetUserByEmail(string email);
    Task AddUser(Users user);
    Task UpdateUser(Users user);
    Task DeleteUser(Users user);
    Task<bool> EmailExists(string email);
}