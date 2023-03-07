namespace PoliticiansAndParties.Api.Contracts.Requests;

public record PoliticianRequest(
    string FullName,
    DateTime BirthDate,
    string ImageUrl,
    string? InstagramUrl = null,
    string? TwitterUrl = null,
    string? FacebookUrl = null);
