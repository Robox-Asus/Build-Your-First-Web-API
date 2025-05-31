namespace TaskManager.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IMemoryTaskRepository Tasks { get; }
        int Complete();
    }
}
