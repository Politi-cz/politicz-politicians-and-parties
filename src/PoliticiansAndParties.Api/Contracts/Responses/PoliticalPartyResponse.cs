namespace PoliticiansAndParties.Api.Contracts.Responses;

public record PoliticalPartyResponse
{
    public Guid Id { get; init; }

#pragma warning disable SA1206 // Declaration keywords should follow order
    public required string Name { get; init; }
#pragma warning restore SA1206 // Declaration keywords should follow order

#pragma warning disable SA1206 // Declaration keywords should follow order
    public required string ImageUrl { get; init; }
#pragma warning restore SA1206 // Declaration keywords should follow order

    public IEnumerable<string> Tags { get; init; } = Enumerable.Empty<string>();

    public IEnumerable<PoliticianResponse> Politicians { get; init; } = Enumerable.Empty<PoliticianResponse>();
}
