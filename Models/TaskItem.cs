using System.ComponentModel.DataAnnotations;

namespace TaskManager_MinimalAPI.Models
{
    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CreatedDateTime { get; set; } = DateTime.Now;
    }
}
