using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Utility
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}