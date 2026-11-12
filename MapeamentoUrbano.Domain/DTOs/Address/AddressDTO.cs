using MapeamentoUrbano.Domain.Validation;

namespace MapeamentoUrbano.Domain.DTOs.Address
{
    public class AddressDTO
    {
        [Guid(AcceptEmptyGuid = true)]
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public AddressDTO()
        {
        }

        public AddressDTO(Entities.Address address)
        {
            Id = address.Id;
            Street = address.Street;
            Number = address.Number;
            Neighborhood = address.Neighborhood;
            City = address.City;
            State = address.State;
            ZipCode = address.ZipCode;
        }

        public Entities.Address MapperToEntity()
        {
            return new Entities.Address
            {
                Id = Id,
                Street = Street,
                Number = Number,
                Neighborhood = Neighborhood,
                City = City,
                State = State,
                ZipCode = ZipCode
            };
        }
    }
}
