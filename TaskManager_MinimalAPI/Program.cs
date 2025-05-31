using EntityFramework_Data;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager_MinimalAPI.Models;
using TaskManager_MinimalAPI.Respositories;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<TaskDbContext>(options=> options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});


// Fix route constraint issue
builder.Services.Configure<RouteOptions>(options => options.ConstraintMap["regex"] = typeof(RegexInlineRouteConstraint));

// Add services to the container.
builder.Services.AddScoped<ITaskRepository, InMemoryTaskRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var tasks = app.MapGroup("/tasks");

// Define the endpoints for the tasks group
tasks.MapGet("/", (ITaskRepository repository) => Results.Ok(repository.GetAllTasks()));
tasks.MapGet("/{id:guid}", (Guid id, ITaskRepository repository) => repository.GetTaskById(id) != null ? Results.Ok(repository.GetTaskById(id)) : Results.NotFound());
tasks.MapPost("/", (TaskItem task, ITaskRepository repository) => 
{
    task.Id = Guid.NewGuid(); // Generate a new ID for the task
    repository.CreateTask(task);
    return Results.Created($"/tasks/{task.Id}", task);
});
tasks.MapPut("/{id:guid}", (Guid id, TaskItem updatedTask, ITaskRepository repository) => 
{
    var existingTask = repository.GetTaskById(id);
    if (existingTask == null)
    {
        return Results.NotFound();
    }
    
    updatedTask.Id = id; // Ensure the ID is set to the existing task's ID
    repository.UpdateTask(updatedTask);
    return Results.Ok(updatedTask);
});
tasks.MapDelete("/{id:guid}", (Guid id, ITaskRepository repository) => 
{
    var task = repository.GetTaskById(id);
    if (task == null)
    {
        return Results.NotFound();
    }
    
    if (repository.DeleteTask(task))
    {
        return Results.Ok(task); // 204 No Content
    }
    
    return Results.BadRequest("Failed to delete the task.");
});
tasks.MapPatch("/{id:guid}", (ITaskRepository repo, Guid id, JsonElement patchData) =>
{
    var task = repo.GetTaskById(id);
    if (task is null) return Results.NotFound();

    // Patch title if present
    if (patchData.TryGetProperty("Title", out var titleProp))
        task.Title = titleProp.GetString() ?? task.Title;

    // Patch description if present
    if (patchData.TryGetProperty("Description", out var descProp))
        task.Description = descProp.GetString() ?? task.Description;

    // Patch isCompleted if present
    if (patchData.TryGetProperty("IsCompleted", out var isCompletedProp))
        task.IsCompleted = isCompletedProp.GetBoolean();

    repo.UpdateTask(task);
    return Results.Ok(task);
});

app.Run();

[JsonSerializable(typeof(IEnumerable<TaskItem>))]
[JsonSerializable(typeof(TaskItem))]
[JsonSerializable(typeof(JsonElement))] 
public partial class AppJsonContext : JsonSerializerContext { }


