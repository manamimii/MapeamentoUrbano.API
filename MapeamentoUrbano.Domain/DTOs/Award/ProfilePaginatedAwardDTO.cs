namespace MapeamentoUrbano.Domain.DTOs.Award
{
    public class ProfilePaginatedAwardDTO
    {
        public DateTime AwardDate { get; set; }
        public string? MedalName { get; set; }
        public int? MedalPoints { get; set; }
        public int Points { get; set; }
        public bool FirstView { get; set; }

        public ProfilePaginatedAwardDTO()
        {
        }

        public ProfilePaginatedAwardDTO(Entities.Award award)
        {
            AwardDate = award.AwardDate;
            MedalName = award.Medal.Name;
            MedalPoints = award.Points;
            Points = award.Points;
            FirstView = award.FirstView;
        }
    }
}
