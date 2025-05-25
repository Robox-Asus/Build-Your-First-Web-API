using TaskManager.Models;

namespace TaskManager.Repositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetAllTasks();
        TaskItem GetTaskById(Guid id);
        TaskItem CreateTask(TaskItem task);
        TaskItem UpdateTask(Guid id, TaskItem task);
        TaskItem DeleteTask(Guid id);
    }
}
