using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class LogRepository : BaseRepository<LogBase>, ILogRepository
    {
        public LogRepository(CoreContext context) : base(context)
        {
        }
    }
}
