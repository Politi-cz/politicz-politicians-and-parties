namespace politicz_politicians_and_parties.Dtos
{
    public class PoliticalPartyCreateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}
