using TaskManager_MinimalAPI.Models;

namespace TaskManager_MinimalAPI.Respositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetAllTasks();
        TaskItem? GetTaskById(Guid id);
        void CreateTask(TaskItem task);
        void UpdateTask(TaskItem task);
        bool DeleteTask(TaskItem task);
    }
}
