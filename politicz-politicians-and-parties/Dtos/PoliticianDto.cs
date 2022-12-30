namespace politicz_politicians_and_parties.Dtos
{
    public class PoliticianDto
    {
        public string FullName { get; set; } = default!;
        public DateTime BirthDate { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? FacebookUrl { get; set; }
    }
}
