using Microsoft.EntityFrameworkCore;
using TaskManager_MinimalAPI.Models;

namespace EntityFramework_Data
{
    public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
    {
        public DbSet<TaskItem> Tasks { get; set; }
    }
}
