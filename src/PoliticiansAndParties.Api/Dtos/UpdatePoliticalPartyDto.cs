namespace PoliticiansAndParties.Api.Dtos;

public class UpdatePoliticalPartyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    public HashSet<string> Tags { get; set; } = new();
}
