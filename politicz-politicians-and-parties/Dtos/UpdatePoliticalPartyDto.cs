namespace politicz_politicians_and_parties.Dtos
{
    public class UpdatePoliticalPartyDto
    {
        public string Name { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public HashSet<string> Tags { get; set; } = new HashSet<string>();
    }
}
