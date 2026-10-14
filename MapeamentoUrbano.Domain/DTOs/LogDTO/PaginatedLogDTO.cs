namespace MapeamentoUrbano.Domain.DTOs.LogDTO
{
    public class PaginatedLogDTO
    {
        public DateTime CreationDate { get; set; }
        public string? Module { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }

        public PaginatedLogDTO()
        {
        }

        public PaginatedLogDTO(Entities.LogBase log)
        {
            CreationDate = log.CreationDate;
            Module = log.Module;
            Action = log.Action;
            UserName = log.User.UserName;
        }
    }
}
