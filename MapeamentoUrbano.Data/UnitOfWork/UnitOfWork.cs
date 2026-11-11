using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.Repository.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;

namespace MapeamentoUrbano.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoreContext _context;
        private bool disposed = false;

        public IAwardRepository AwardRepository { get; set; }
        public IAddressRepository AddressRepository { get; set; }
        public IEventRepository EventRepository { get; set; }
        public ILogRepository LogRepository { get; set; }
        public IMedalsRepository MedalsRepository { get; set; }
        public IRoleRepository RoleRepository { get; set; }
        public IUserRepository UserRepository { get; set; }

        public UnitOfWork(CoreContext context)
        {
            _context = context;
            this.AwardRepository = new AwardRepository(_context);
            this.AddressRepository = new AddressRepository(_context);
            this.EventRepository = new EventRepository(_context);
            this.LogRepository = new LogRepository(_context);
            this.MedalsRepository = new MedalsRepository(_context);
            this.RoleRepository = new RoleRepository(_context);
            this.UserRepository = new UserRepository(_context);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
                _context.Dispose();

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public dynamic GetContext()
        {
            return _context;
        }
    }
}
