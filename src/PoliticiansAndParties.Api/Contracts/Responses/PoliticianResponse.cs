namespace PoliticiansAndParties.Api.Contracts.Responses;
// public class PoliticianResponse
// {
//     public Guid Id { get; set; }
//     public string FullName { get; set; } = default!;
//     public DateTime BirthDate { get; set; }
//     public string? InstagramUrl { get; set; }
//     public string? TwitterUrl { get; set; }
//     public string? FacebookUrl { get; set; }
// }

public record PoliticianResponse(
    Guid Id,
    string FullName,
    DateTime BirthDate,
    string ImageUrl,
    string? InstagramUrl = null,
    string? TwitterUrl = null,
    string? FacebookUrl = null);
