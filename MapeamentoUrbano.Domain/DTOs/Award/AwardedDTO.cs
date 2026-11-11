namespace MapeamentoUrbano.Domain.DTOs.Award
{
    public class AwardedDTO
    {
        public Guid UserId { get; set; }
        public Guid MedalId { get; set; }
        public int Points { get; set; }

        public AwardedDTO()
        {
        }

        public AwardedDTO(Entities.Award award)
        {
            UserId = award.UserId;
            MedalId = award.MedalId;
            Points = award.Points;
        }
    }
}
