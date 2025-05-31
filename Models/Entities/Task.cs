namespace TaskManager.Domain.Entities;

public record TaskDto(Guid Id, string Title, string Description, bool IsCompleted);
public record CreateOrUpdateTaskDto(string Title, string Description, bool IsCompleted);

