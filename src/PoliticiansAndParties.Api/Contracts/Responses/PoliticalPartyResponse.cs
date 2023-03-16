namespace PoliticiansAndParties.Api.Contracts.Responses;

public record PoliticalPartyResponse(
    Guid Id,
    string Name,
    string ImageUrl,
    IEnumerable<string> Tags,
    IEnumerable<PoliticianResponse> Politicians);
