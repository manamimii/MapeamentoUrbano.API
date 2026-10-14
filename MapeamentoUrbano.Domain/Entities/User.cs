using MapeamentoUrbano.Domain.DTOs.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapeamentoUrbano.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(250)]
        public string UserName { get; set; }
        public string UserNameNormalized { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserPassword { get; set; }
        public bool FirstAccess { get; set; }
        public Guid RoleId { get; set; }

        #region [Foreign Key]
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        #endregion

        public User MapperToEntity(UserDTO userDTO)
        {
            Id = userDTO.Id;
            UserName = userDTO.UserName;
            UserEmail = userDTO.UserEmail;
            return this;
        }
    }
}
