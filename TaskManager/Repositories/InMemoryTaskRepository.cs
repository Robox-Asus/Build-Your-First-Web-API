using TaskManager.Models;

namespace TaskManager.Repositories
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new();
        public IEnumerable<TaskItem> GetAllTasks() => _tasks;
        public TaskItem GetTaskById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id) ?? new TaskItem { Title = $"Task {id} Not Found", Description = $"Task {id} Not Found" };
        public TaskItem CreateTask(TaskItem task)
        {
            task.Id = Guid.NewGuid();
            _tasks.Add(task);
            return task;
        }
        public TaskItem DeleteTask(Guid id)
        {
            var existingTask = GetTaskById(id);
            if (existingTask == null)
            {
                return new TaskItem { Title = $"Task {id} Not Found", Description = $"Task {id} Not Found" };
            }
            _tasks.Remove(existingTask);
            return existingTask;
        }
        public TaskItem UpdateTask(Guid id, TaskItem task)
        {
            var index = _tasks.FindIndex(t => t.Id == id);
            if (index == -1)
            {
                return new TaskItem() { Title = $"Task {id} Not Found", Description = $"Task {id} Not Found" };
            }
            else
            {
                _tasks[index] = task;
                return task;
            }
        }
    }
}
