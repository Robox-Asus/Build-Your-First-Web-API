using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Respositories
{
    public class UnitOfWork(TaskDbContext _context,IMemoryTaskRepository _task) : IUnitOfWork
    {
        public IMemoryTaskRepository Tasks => _task;
        public int Complete() => _context.SaveChanges();
    }
}
