using MapeamentoUrbano.Domain.Entities;

namespace MapeamentoUrbano.Domain.DTOs.Award
{
    public class AwardDTO
    {
        public DateTime AwardDate { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalAwards { get; set; }
        public List<AwardedDTO> Awards { get; set; }

        public AwardDTO()
        {
        }

        public AwardDTO(DateTime date, int totalParticipants, List<Entities.Award> awards)
        {
            AwardDate = date;
            TotalParticipants = totalParticipants;
            TotalAwards = awards.Count;
            Awards = awards.Select(award => new AwardedDTO(award)).ToList();
        }
    }
}
