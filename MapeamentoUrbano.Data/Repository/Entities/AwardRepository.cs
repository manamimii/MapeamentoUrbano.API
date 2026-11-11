using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class AwardRepository : BaseRepository<Award>, IAwardRepository
    {
        public AwardRepository(CoreContext context) : base(context)
        {
        }
    }
}
