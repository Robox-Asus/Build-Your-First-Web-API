using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManager_MinimalAPI.Models;
using TaskManager_MinimalAPI.Respositories;

namespace TaskManager.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskRepository repository) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetAllTasks() => Ok(repository.GetAllTasks());
        [HttpGet("{id:guid}")]
        public ActionResult<TaskItem> GetTaskById(Guid id) => Ok(repository.GetTaskById(id) ?? new TaskItem() { Title = "Task Not Found", Description = "Task Not Found" });
        [HttpPost]
        public ActionResult<TaskItem> CreateTask(TaskItem task)
        {
            task.Id = Guid.NewGuid(); // Generate a new ID for the task
            repository.CreateTask(task);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        [HttpPut("{id:guid}")]
        public ActionResult<TaskItem> UpdateTask(Guid id, TaskItem updatedTask)
        {
            var existingTask = repository.GetTaskById(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            updatedTask.Id = id; // Ensure the ID is set to the existing task's ID
            repository.UpdateTask(updatedTask);
            return Ok(updatedTask);
        }
        [HttpDelete]
        public ActionResult<TaskItem> DeleteTask(Guid id)
        {
            var task = repository.GetTaskById(id);
            if (task == null)
            {
                return NotFound();
            }
            if (repository.DeleteTask(task))
            {
                return Ok(task); // 204 No Content
            }
            return BadRequest("Failed to delete the task.");
        }
        [HttpPatch("{id:guid}")]
        public ActionResult<TaskItem> PatchTask(Guid id, [FromBody] JsonElement patchData)
        {
            var task = repository.GetTaskById(id);
            if (task is null) return NotFound();
            // Patch title if present
            if (patchData.TryGetProperty("Title", out var titleProp))
                task.Title = titleProp.GetString() ?? task.Title;
            // Patch description if present
            if (patchData.TryGetProperty("Description", out var descProp))
                task.Description = descProp.GetString() ?? task.Description;
            // Patch isCompleted if present
            if (patchData.TryGetProperty("IsCompleted", out var isCompletedProp))
                task.IsCompleted = isCompletedProp.GetBoolean();
            repository.UpdateTask(task);
            return Ok(task);
        }

    }
}
