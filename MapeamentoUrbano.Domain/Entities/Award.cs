using System.ComponentModel.DataAnnotations.Schema;

namespace MapeamentoUrbano.Domain.Entities
{
    public class Award : BaseEntity
    {
        public DateTime AwardDate { get; set; }
        public int Points { get; set; }
        public bool FirstView { get; set; }
        public Guid UserId { get; set; }
        public Guid MedalId { get; set; }


        #region [Foreign Keys]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("MedalId")]
        public virtual Medals Medal { get; set; }
        #endregion
    }
}
