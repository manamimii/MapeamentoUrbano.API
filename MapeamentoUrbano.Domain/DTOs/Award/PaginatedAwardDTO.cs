namespace MapeamentoUrbano.Domain.DTOs.Award
{
    public class PaginatedAwardDTO
    {
        public DateTime AwardDate { get; set; }
        public string? UserGold { get; set; }
        public int? UserGoldPoints { get; set; }
        public string? UserSilver { get; set; }
        public int? UserSilverPoints { get; set; }
        public string? UserBronze { get; set; }
        public int? UserBronzePoints { get; set; }
    }
}
