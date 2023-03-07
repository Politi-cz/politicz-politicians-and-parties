namespace PoliticiansAndParties.Api.Contracts.Responses;

public record PoliticalPartyResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public IEnumerable<string> Tags { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<PoliticianResponse> Politicians { get; init; } = Enumerable.Empty<PoliticianResponse>();
}
