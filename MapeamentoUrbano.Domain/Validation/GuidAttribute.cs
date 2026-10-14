using System.ComponentModel.DataAnnotations;

namespace MapeamentoUrbano.Domain.Validation
{
    public class GuidAttribute : ValidationAttribute
    {
        public GuidAttribute() { }

        public bool AcceptEmptyGuid { get; set; } = false;

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return null;

            if (!Guid.TryParse(value.ToString(), out Guid guidParse))
                return new ValidationResult("O valor enviado não é uma Guid válida.");

            if (!AcceptEmptyGuid && Guid.Empty == (Guid)guidParse)
                return new ValidationResult("Guid não pode ser vazia.");

            return null;
        }
    }
}