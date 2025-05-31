using TaskManager.Domain.Entities;
using TaskManager_MinimalAPI.Respositories;

namespace TaskManager.Domain.Interfaces
{
    public interface IMemoryTaskRepository : ITaskRepository<TaskItem>
    {
    }
}
