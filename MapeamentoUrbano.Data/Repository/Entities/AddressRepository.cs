using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.Repository.Entities;

namespace MapeamentoUrbano.Data.Repository.Entities
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        public AddressRepository(CoreContext context) : base(context)
        {
        }
    }
}
