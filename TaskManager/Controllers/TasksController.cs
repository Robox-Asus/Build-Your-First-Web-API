using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(IUnitOfWork _work,IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllTasks() => Ok(_mapper.Map<IEnumerable<TaskDto>>(_work.Tasks.GetAllTasks()));
        [HttpGet("{id:guid}")]
        public IActionResult GetTaskById(Guid id)
        {
            var task = _work.Tasks.GetTaskById(id);
            return task is null ? NotFound() : Ok(_mapper.Map<TaskDto>(task));
        }
        [HttpPost]
        public IActionResult CreateTask(CreateOrUpdateTaskDto task)
        {
            var newTask = _mapper.Map<TaskItem>(task);
            newTask.Id = Guid.NewGuid(); // Generate a new ID for the task
            _work.Tasks.CreateTask(newTask);
            _work.Complete(); // Save changes to the database
            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id }, _mapper.Map<TaskDto>(newTask));
        }
        [HttpPut("{id:guid}")]
        public IActionResult UpdateTask(Guid id, CreateOrUpdateTaskDto updatedTask)
        {
            var existingTask = _work.Tasks.GetTaskById(id);
            if (existingTask == null) return NotFound();
            else
            {
                var modifyTask = _mapper.Map<TaskItem>(updatedTask);
                modifyTask.Id = id; // Ensure the ID is set to the existing task's ID
                _work.Tasks.UpdateTask(modifyTask);
                _work.Complete(); // Save changes to the database
                return Ok(existingTask);
            }
        }
        [HttpDelete]
        public IActionResult DeleteTask(Guid id)
        {
            var task = _work.Tasks.GetTaskById(id);
            if (task == null) return NotFound();
            try
            {
                _work.Tasks.DeleteTask(task);
                _work.Complete(); // Save changes to the database
                return NoContent(); // Return 204 No Content on successful deletion
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return BadRequest($"Failed to delete the task.${ex.ToString()} .");
            }
        }
        [HttpPatch("{id:guid}")]
        public IActionResult PatchTask(Guid id, [FromBody] JsonElement patchData)
        {
            var task = _work.Tasks.GetTaskById(id);
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
            // Update the task in the repository
            try
            {
                _work.Tasks.UpdateTask(task);
                return Ok(_mapper.Map<TaskDto>(task));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return BadRequest($"Failed to update the task.${ex.ToString()} .");
            }
        }

    }
}
