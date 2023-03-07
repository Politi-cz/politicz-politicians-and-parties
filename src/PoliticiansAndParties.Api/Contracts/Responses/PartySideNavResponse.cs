namespace PoliticiansAndParties.Api.Contracts.Responses;

public record PartySideNavResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public IEnumerable<string> Tags { get; init; } = Enumerable.Empty<string>();
}
