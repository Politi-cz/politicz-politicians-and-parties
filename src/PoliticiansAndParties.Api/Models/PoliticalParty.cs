namespace PoliticiansAndParties.Api.Models;

public class PoliticalParty
{
    public int Id { get; set; } = default!;

    public Guid FrontEndId { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public List<Politician> Politicians { get; set; } = new();

    public HashSet<string> Tags { get; set; } = new();
}
