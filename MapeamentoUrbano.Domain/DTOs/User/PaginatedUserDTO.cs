using MapeamentoUrbano.Domain.Validation;

namespace MapeamentoUrbano.Domain.DTOs.User
{
    public class PaginatedUserDTO
    {
        [Guid(AcceptEmptyGuid = true)]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        public PaginatedUserDTO()
        {
        }

        public PaginatedUserDTO(Entities.User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            UserEmail = user.UserEmail;
            UserRole = user.Role.RoleName;
        }
    }
}
