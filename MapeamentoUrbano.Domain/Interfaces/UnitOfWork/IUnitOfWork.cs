using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository EventRepository { get; }
        ILogRepository LogRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }

        int Commit();
        dynamic GetContext();
    }
}
