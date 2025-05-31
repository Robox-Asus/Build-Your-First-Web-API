using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager_MinimalAPI.Respositories
{
    public class MemoryTaskRepository(TaskDbContext context) : IMemoryTaskRepository
    {
        // Use AsNoTracking for read-only queries to improve performance
        public IEnumerable<TaskItem> GetAllTasks() => context.Tasks.AsNoTracking().ToList();

        public TaskItem? GetTaskById(Guid id) =>
            context.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == id);

        public void CreateTask(TaskItem task) => context.Tasks.Add(task);

        public void DeleteTask(TaskItem task) => context.Tasks.Remove(task);

        public void UpdateTask(TaskItem task)
        {
            // Attach only if not already tracked to avoid unnecessary state changes
            if (context.Entry(task).State == EntityState.Detached)
            {
                context.Tasks.Attach(task);
            }
            context.Entry(task).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
