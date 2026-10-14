using System.ComponentModel.DataAnnotations;

namespace MapeamentoUrbano.Domain.DTOs.Login
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public LoginDTO()
        {
        }

        public LoginDTO(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
