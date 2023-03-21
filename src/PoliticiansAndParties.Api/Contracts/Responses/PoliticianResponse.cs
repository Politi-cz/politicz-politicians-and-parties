namespace PoliticiansAndParties.Api.Contracts.Responses;

public record PoliticianResponse(
    Guid Id,
    string FullName,
    DateTime BirthDate,
    string ImageUrl,
    string? InstagramUrl = null,
    string? TwitterUrl = null,
    string? FacebookUrl = null);
