using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Utility;

namespace TaskManager.Infrastructure.Respositories
{
    public class UnitOfWork(TaskDbContext _context,IMemoryTaskRepository _task,IUserRepository userRepository,IJwtService jwtService) : IUnitOfWork
    {
        public IMemoryTaskRepository Tasks => _task;

        public IUserRepository UserRepository => userRepository;

        public IJwtService JwtService => jwtService;

        public int Complete() => _context.SaveChanges();
    }
}
