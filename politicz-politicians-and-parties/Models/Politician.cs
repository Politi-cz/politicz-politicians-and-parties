namespace politicz_politicians_and_parties.Models
{
    public class Politician
    {
        public int Id { get; set; } = default!;
        public Guid FrontEndId { get; set; }
        public string FullName { get; set; } = default!;
        public DateTime BirthDate { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public int PoliticalPartyId { get; set; } = default!;
    }
}
