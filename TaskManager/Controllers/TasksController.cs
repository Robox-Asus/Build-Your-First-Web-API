using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskRepository repository) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetAllTasks() => Ok(repository.GetAllTasks());
        [HttpGet("{id:guid}")]
        public ActionResult<TaskItem> GetTaskById(Guid id) => Ok(repository.GetTaskById(id) ?? new TaskItem() { Title= "Task Not Found", Description ="Task Not Found" } );
        [HttpPost]
        public ActionResult<TaskItem> CreateTask(TaskItem task) => Ok(repository.CreateTask(task));
        [HttpPut("{id:guid}")]
        public ActionResult<TaskItem> UpdateTask(Guid id, TaskItem task) => Ok(repository.UpdateTask(id, task));
        [HttpDelete]
        public ActionResult<TaskItem> DeleteTask(Guid id) => Ok(repository.DeleteTask(id));

    }
}
