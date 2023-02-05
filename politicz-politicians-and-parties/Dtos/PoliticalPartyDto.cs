namespace politicz_politicians_and_parties.Dtos
{
    public class PoliticalPartyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public HashSet<string> Tags { get; set; } = new HashSet<string>();

        public List<PoliticianDto> Politicians { get; set; } = new List<PoliticianDto>();
    }
}
