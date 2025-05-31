using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Respositories;
using TaskManager.Infrastructure.Utility;
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
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IMemoryTaskRepository, MemoryTaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();
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
//get all tasks
tasks.MapGet("/", ([FromServices] IUnitOfWork _work, [FromServices] IMapper _mapper) => Results.Ok(_mapper.Map<IEnumerable<TaskDto>>(_work.Tasks.GetAllTasks())));

//get task by id
tasks.MapGet("/{id:guid}", (Guid id, [FromServices] IUnitOfWork _work, [FromServices] IMapper _mapper) => {
    var task = _work.Tasks.GetTaskById(id);
    return task is null ? Results.NotFound() : Results.Ok(_mapper.Map<TaskDto>(task));
});

// Create a new task
tasks.MapPost("/", (CreateOrUpdateTaskDto task, [FromServices] IUnitOfWork _work, [FromServices] IMapper _mapper) => {
    var newTask = _mapper.Map<TaskItem>(task);
    newTask.Id = Guid.NewGuid(); // Generate a new ID for the task
    _work.Tasks.CreateTask(newTask);
    _work.Complete(); // Save changes to the database
    return Results.Created($"/tasks/{newTask.Id}", _mapper.Map<TaskDto>(newTask));
});

// Update an existing task
tasks.MapPut("/{id:guid}", (Guid id,CreateOrUpdateTaskDto task, [FromServices] IUnitOfWork _work, [FromServices] IMapper _mapper) => 
{
    var existingTask = _work.Tasks.GetTaskById(id);
    if (existingTask == null) return Results.NotFound();
    else
    {
        var modifyTask = _mapper.Map<TaskItem>(task);
        modifyTask.Id = id; // Ensure the ID is set to the existing task's ID
        _work.Tasks.UpdateTask(modifyTask);
        _work.Complete(); // Save changes to the database
        return Results.Ok(_mapper.Map<TaskDto>(existingTask));
    }
});

// Delete a task by ID
tasks.MapDelete("/{id:guid}", (Guid id, [FromServices] IUnitOfWork _work) => 
{
    var task = _work.Tasks.GetTaskById(id);
    if (task == null) return Results.NotFound();

    try
    {
        _work.Tasks.DeleteTask(task);
        _work.Complete(); // Save changes to the database
        return Results.NoContent(); // Return 204 No Content on successful deletion
    }
    catch (Exception ex)
    {
        // Log the exception if needed
        return Results.BadRequest($"Failed to delete the task.${ex.ToString()} .");
    }
});

// Patch a task by ID
tasks.MapPatch("/{id:guid}", (Guid id, JsonElement patchData, [FromServices] IUnitOfWork _work, [FromServices] IMapper _mapper) =>
{
    var task = _work.Tasks.GetTaskById(id);
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

    // Update the task in the repository
    try
    {
        _work.Tasks.UpdateTask(task);
        return Results.Ok(_mapper.Map<TaskDto>(task));
    }
    catch (Exception ex)
    {
        // Log the exception if needed
        return Results.BadRequest($"Failed to update the task.${ex.ToString()} .");
    }
});

app.Run();

[JsonSerializable(typeof(IEnumerable<TaskItem>))]
[JsonSerializable(typeof(TaskItem))]
[JsonSerializable(typeof(JsonElement))] 
public partial class AppJsonContext : JsonSerializerContext { }


