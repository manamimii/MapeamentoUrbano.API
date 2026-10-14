using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(CoreContext context) : base(context)
        {
        }
    }
}
