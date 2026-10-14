using System.ComponentModel.DataAnnotations;

namespace MapeamentoUrbano.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
