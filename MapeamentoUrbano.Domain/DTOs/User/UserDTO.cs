using MapeamentoUrbano.Domain.Validation;

namespace MapeamentoUrbano.Domain.DTOs.User
{
    public class UserDTO
    {
        [Guid(AcceptEmptyGuid = true)]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }

        public UserDTO()
        {
        }

        public UserDTO(Entities.User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            UserEmail = user.UserEmail;
        }

        public Entities.User MapperToEntity()
        {
            return new Entities.User
            {
                Id = Id,
                UserName = UserName,
                UserEmail = UserEmail

            };
        }
    }
}
