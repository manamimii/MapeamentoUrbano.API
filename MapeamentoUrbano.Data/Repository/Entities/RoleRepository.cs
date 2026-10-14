using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(CoreContext context) : base(context)
        {
        }
    }
}
