namespace politicz_politicians_and_parties.Contracts.Responses
{
    // public class PoliticianResponse
    // {
    //     public Guid Id { get; set; }
    //     public string FullName { get; set; } = default!;
    //     public DateTime BirthDate { get; set; }
    //     public string? InstagramUrl { get; set; }
    //     public string? TwitterUrl { get; set; }
    //     public string? FacebookUrl { get; set; }
    // }

    public record PoliticianResponse(Guid Id, string FullName, DateTime BirthDate, string? InstagramUrl,
        string? TwitterUrl, string? FacebookUrl);
}
