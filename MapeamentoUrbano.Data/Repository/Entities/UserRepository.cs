using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CoreContext context) : base(context)
        {
        }
    }
}
