using MapeamentoUrbano.Domain.DTOs.Address;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapeamentoUrbano.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid UserId { get; set; }

        #region [Foreign Keys]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        #endregion

        public Address MapperToEntity(AddressDTO addressDTO)
        {
            Id = addressDTO.Id;
            Street = addressDTO.Street;
            Number = addressDTO.Number;
            Neighborhood = addressDTO.Neighborhood;
            City = addressDTO.City;
            State = addressDTO.State;
            ZipCode = addressDTO.ZipCode;
            return this;
        }
    }
}
