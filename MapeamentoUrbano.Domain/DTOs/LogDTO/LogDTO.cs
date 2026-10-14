namespace MapeamentoUrbano.Domain.DTOs.LogDTO
{
    public class LogDTO
    {
        public DateTime CreationDate { get; set; }
        public string? Module { get; set; }
        public string Action { get; set; }
        public Guid UserId { get; set; }

        public LogDTO()
        {
            CreationDate = DateTime.Now;
        }

        public LogDTO(string module, string action, Guid userId)
        {
            CreationDate = DateTime.Now;
            Module = module;
            Action = action;
            UserId = userId;
        }
    }
}
