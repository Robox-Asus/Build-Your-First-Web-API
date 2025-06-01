using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Respositories
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User GetUserByUsername(string username);
    }
}