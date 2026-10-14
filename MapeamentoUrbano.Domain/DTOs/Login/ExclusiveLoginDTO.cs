using System.ComponentModel.DataAnnotations;

namespace MapeamentoUrbano.Domain.DTOs.Login
{
    public class ExclusiveLoginDTO
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        public ExclusiveLoginDTO()
        {
        }

        public ExclusiveLoginDTO(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
