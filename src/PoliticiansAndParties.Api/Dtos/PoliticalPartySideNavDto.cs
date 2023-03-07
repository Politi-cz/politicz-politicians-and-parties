namespace PoliticiansAndParties.Api.Dtos;

public class PoliticalPartySideNavDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
}
