using MapeamentoUrbano.Domain.Validation;

namespace MapeamentoUrbano.Domain.DTOs.Event
{
    public class EventDTO
    {
        [Guid(AcceptEmptyGuid = true)]
        public Guid Id { get; set; }
        public byte[] Photo { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Description { get; set; }

        public EventDTO()
        {
        }

        public EventDTO(Entities.Event eventEntity)
        {
            Id = eventEntity.Id;
            Photo = eventEntity.Photo;
            Latitude = eventEntity.Latitude;
            Longitude = eventEntity.Longitude;
            Description = eventEntity.Description;
        }

        public Entities.Event MapperToEntity()
        {
            return new Entities.Event
            {
                Photo = this.Photo,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Description = this.Description,
                RegistrationDate = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }
    }
}
