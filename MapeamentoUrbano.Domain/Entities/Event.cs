using MapeamentoUrbano.Domain.DTOs.Event;
using MapeamentoUrbano.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapeamentoUrbano.Domain.Entities
{
    public class Event : BaseEntity
    {
        public byte[] Photo { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime UpdateTime { get; set; }
        public Guid UserId { get; set; }

        #region [Foreign Keys]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        #endregion

        public Event() { }

        public Event(DTOs.Event.EventDTO eventDTO)
        {
            Photo = eventDTO.Photo;
            Latitude = eventDTO.Latitude;
            Longitude = eventDTO.Longitude;
            Description = eventDTO.Description;
            RegistrationDate = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
        }

        public DTOs.Event.EventDTO MapperToEntity(EventDTO eventDTO)
        {
            return new DTOs.Event.EventDTO
            {
                Photo = eventDTO.Photo,
                Latitude = eventDTO.Latitude,
                Longitude = eventDTO.Longitude,
                Description = eventDTO.Description
            };
        }
    }
}
