using Calendar.Models;

namespace Calendar.Repositories;

public interface ITasksRepo
{
    Task<List<Tasks>> GetTasksByUser(int userId);
    Task<Tasks> AddTask(Tasks task);
    Task<Tasks?> UpdateTask(Tasks task);
    Task<bool> DeleteTask(int taskId);
    Task<bool> CheckTaskConflict(int userId, DateTime start, DateTime end, int? excludeId = null);
}