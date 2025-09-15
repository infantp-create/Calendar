using Calendar.Data;
using Calendar.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Repositories;

public class TasksRepo : ITasksRepo
{
    private readonly AppDbContext _context;
    public TasksRepo(AppDbContext context) => _context = context;

    public async Task<List<Tasks>> GetTasksByUser(int userId)
        => await _context.Tasks.Where(t => t.UserId == userId).OrderBy(t => t.Start).ToListAsync();

    public async Task<Tasks> AddTask(Tasks task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Tasks?> UpdateTask(Tasks task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTask(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckTaskConflict(int userId, DateTime start, DateTime end, int? excludeId = null)
    {
        return await _context.Tasks.AnyAsync(t =>
            t.UserId == userId &&
            (excludeId == null || t.Id != excludeId) &&
            t.Start < end && t.End > start
        );
    }
}