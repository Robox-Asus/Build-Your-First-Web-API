using TaskManager.Infrastructure.Respositories;
using TaskManager.Infrastructure.Utility;

namespace TaskManager.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IMemoryTaskRepository Tasks { get; }
        int Complete();
        IUserRepository UserRepository { get; }
        IJwtService JwtService { get; }
    }
}
