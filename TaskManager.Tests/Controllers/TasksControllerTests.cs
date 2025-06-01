namespace TaskManager.Tests.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Moq;
using TaskManager.Domain.Interfaces;
using TaskManager.Controllers;
using TaskManager.Domain.Entities;

public class TasksControllerTests
{
    private readonly Mock<IUnitOfWork> _mockWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _mockWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _controller = new TasksController(_mockWork.Object, _mockMapper.Object);
    }

    [Fact]
    public void GetAllTasks_ReturnsOkWithTasks()
    {
        var tasks = new List<TaskItem> { new TaskItem() };
        var taskDtos = new List<TaskDto> { new TaskDto(Guid.NewGuid(), "Title", "Description", false) };
        _mockWork.Setup(w => w.Tasks.GetAllTasks()).Returns(tasks);
        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(tasks)).Returns(taskDtos);

        var result = _controller.GetAllTasks();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(taskDtos, okResult.Value);
    }

    [Fact]
    public void GetTaskById_TaskExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = id };
        var taskDto = new TaskDto(id, "Title", "Description", false);
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns(task);
        _mockMapper.Setup(m => m.Map<TaskDto>(task)).Returns(taskDto);

        var result = _controller.GetTaskById(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(taskDto, okResult.Value);
    }

    [Fact]
    public void GetTaskById_TaskNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns((TaskItem?)null);

        var result = _controller.GetTaskById(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void UpdateTask_TaskExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var existingTask = new TaskItem { Id = id };
        var dto = new CreateOrUpdateTaskDto("Title", "Description", false);
        var modifyTask = new TaskItem { Id = id };
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns(existingTask);
        _mockMapper.Setup(m => m.Map<TaskItem>(dto)).Returns(modifyTask);

        var result = _controller.UpdateTask(id, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(existingTask, okResult.Value);
        _mockWork.Verify(w => w.Tasks.UpdateTask(modifyTask), Times.Once);
        _mockWork.Verify(w => w.Complete(), Times.Once);
    }

    [Fact]
    public void UpdateTask_TaskNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new CreateOrUpdateTaskDto("Title", "Description", false);
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns((TaskItem?)null);

        var result = _controller.UpdateTask(id, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteTask_TaskExists_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = id };
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns(task);

        var result = _controller.DeleteTask(id);

        Assert.IsType<NoContentResult>(result);
        _mockWork.Verify(w => w.Tasks.DeleteTask(task), Times.Once);
        _mockWork.Verify(w => w.Complete(), Times.Once);
    }

    [Fact]
    public void DeleteTask_TaskNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns((TaskItem?)null);

        var result = _controller.DeleteTask(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteTask_Exception_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = id };
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns(task);
        _mockWork.Setup(w => w.Tasks.DeleteTask(task)).Throws(new Exception("fail"));

        var result = _controller.DeleteTask(id);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Failed to delete the task", badRequest.Value?.ToString());
    }

    [Fact]
    public void PatchTask_TaskExists_UpdatesFields()
    {
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = id, Title = "Old", Description = "Old", IsCompleted = false };
        var patchJson = "{\"Title\":\"New\",\"Description\":\"Desc\",\"IsCompleted\":true}";
        var patchData = JsonDocument.Parse(patchJson).RootElement;
        var taskDto = new TaskDto(id, "New", "Desc", true);
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns(task);
        _mockMapper.Setup(m => m.Map<TaskDto>(task)).Returns(taskDto);

        var result = _controller.PatchTask(id, patchData);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(taskDto, okResult.Value);
        Assert.Equal("New", task.Title);
        Assert.Equal("Desc", task.Description);
        Assert.True(task.IsCompleted);
        _mockWork.Verify(w => w.Tasks.UpdateTask(task), Times.Once);
    }

    [Fact]
    public void PatchTask_TaskNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var patchData = JsonDocument.Parse("{}").RootElement;
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns((TaskItem?)null);

        var result = _controller.PatchTask(id, patchData);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void PatchTask_Exception_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = id };
        var patchData = JsonDocument.Parse("{}").RootElement;
        _mockWork.Setup(w => w.Tasks.GetTaskById(id)).Returns(task);
        _mockWork.Setup(w => w.Tasks.UpdateTask(task)).Throws(new Exception("fail"));

        var result = _controller.PatchTask(id, patchData);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Failed to update the task", badRequest.Value?.ToString());
    }
}