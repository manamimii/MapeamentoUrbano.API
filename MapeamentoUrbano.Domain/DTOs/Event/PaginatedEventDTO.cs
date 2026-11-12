using MapeamentoUrbano.Domain.Validation;

namespace MapeamentoUrbano.Domain.DTOs.Event
{
    public class PaginatedEventDTO
    {
        [Guid(AcceptEmptyGuid = true)]
        public Guid Id { get; set; }
        public byte[] Photo { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public int Status { get; set; }
        public string? User { get; set; }

        public PaginatedEventDTO()
        {
        }

        public PaginatedEventDTO(Entities.Event evt)
        {
            Id = evt.Id;
            Photo = evt.Photo;
            Description = evt.Description;
            Points = evt.Points;
            Status = evt.Status;
            User = evt.User.UserName;
        }
    }
}
