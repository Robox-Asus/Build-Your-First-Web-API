namespace TaskManager.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IsCompleted { get; set; } = "false"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
