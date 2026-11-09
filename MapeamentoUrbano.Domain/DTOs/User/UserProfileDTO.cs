namespace MapeamentoUrbano.Domain.DTOs.User
{
    public class UserProfileDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public UserProfileDTO()
        {
        }

        public UserProfileDTO(Entities.User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            UserEmail = user.UserEmail;
        }
    }
}
