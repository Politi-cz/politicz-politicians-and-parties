namespace PoliticiansAndParties.Api.Contracts.Requests;

public record PoliticalPartyRequest
{
#pragma warning disable SA1206
    public required string Name { get; init; }

#pragma warning restore SA1206
#pragma warning disable SA1206
    public required string ImageUrl { get; init; }

#pragma warning restore SA1206
    public HashSet<string> Tags { get; init; } = new();

    public List<PoliticianRequest> Politicians { get; init; } = new();
}
