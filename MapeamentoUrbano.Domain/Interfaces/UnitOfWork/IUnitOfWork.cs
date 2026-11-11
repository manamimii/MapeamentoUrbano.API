using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAwardRepository AwardRepository { get; }
        IAddressRepository AddressRepository { get; }
        IEventRepository EventRepository { get; }
        ILogRepository LogRepository { get; }
        IMedalsRepository MedalsRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }

        int Commit();
        dynamic GetContext();
    }
}
