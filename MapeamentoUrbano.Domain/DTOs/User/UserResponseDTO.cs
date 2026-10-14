using MapeamentoUrbano.Domain.Validation;

namespace MapeamentoUrbano.Domain.DTOs.User
{
    public class UserResponseDTO
    {
        [Guid(AcceptEmptyGuid = true)]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }

        public UserResponseDTO()
        {
        }

        public UserResponseDTO(Entities.User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            UserEmail = user.UserEmail;
        }
    }
}
