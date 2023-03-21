namespace politicz_politicians_and_parties.Contracts.Requests
{
    // public class PoliticianRequest
    // {
    //     public string FullName { get; set; } = default!;
    //     public DateTime BirthDate { get; set; }
    //     public string? InstagramUrl { get; set; }
    //     public string? TwitterUrl { get; set; }
    //     public string? FacebookUrl { get; set; }
    // }

    public record PoliticianRequest(string FullName, DateTime BirthDate, string? InstagramUrl = null, string? TwitterUrl = null,
        string? FacebookUrl = null);
}
