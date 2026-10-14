using MapeamentoUrbano.Domain.DTOs.LogDTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapeamentoUrbano.Domain.Entities
{
    public class LogBase : BaseEntity
    {
        public DateTime CreationDate { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public Guid UserId { get; set; }

        #region [Foreign Key]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        #endregion

        public LogBase()
        {
        }

        public LogBase(LogDTO logDTO)
        {
            CreationDate = logDTO.CreationDate;
            Module = logDTO.Module;
            Action = logDTO.Action;
            UserId = logDTO.UserId;
        }
    }
}
