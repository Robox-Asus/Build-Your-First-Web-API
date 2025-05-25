using Microsoft.AspNetCore.Mvc;
using TaskManager_MinimalAPI.Models;

namespace TaskManager_MinimalAPI.Respositories
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        public readonly List<TaskItem> _tasks = new();
        public IEnumerable<TaskItem> GetAllTasks() => _tasks;
        public TaskItem? GetTaskById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id) ?? null;
        public void CreateTask(TaskItem task) => _tasks.Add(task);
        public bool DeleteTask(TaskItem task) => _tasks.Remove(task) ;
        public void UpdateTask(TaskItem task) => _tasks[_tasks.FindIndex(t => t.Id == task.Id)] = task;
    }
}
