namespace politicz_politicians_and_parties.Models
{
    public class PoliticalParty
    {
        public int Id { get; set; } = default!;
        public Guid FrontEndId { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public IEnumerable<Politician> Politicians { get; set; } = Enumerable.Empty<Politician>();
    }
}
