namespace PoliticiansAndParties.Api.Contracts.Responses;

public record PartySideNavResponse(
    Guid Id,
    string Name,
    string ImageUrl,
    IEnumerable<string> Tags);
