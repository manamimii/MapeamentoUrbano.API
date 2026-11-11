using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class MedalsRepository : BaseRepository<Medals>, IMedalsRepository
    {
        public MedalsRepository(CoreContext context) : base(context)
        {
        }
    }
}
