using TaskManager.Domain.Entities;

namespace TaskManager_MinimalAPI.Respositories
{
    public interface ITaskRepository<T> where T : class
    {
        IEnumerable<T> GetAllTasks();
        T? GetTaskById(Guid id);
        void CreateTask(T task);
        void UpdateTask(T task);
        void DeleteTask(T task);
    }
}
