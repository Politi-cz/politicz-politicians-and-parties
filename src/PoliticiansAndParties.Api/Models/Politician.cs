namespace PoliticiansAndParties.Api.Models;

public class Politician
{
    public int Id { get; set; }
    public Guid FrontEndId { get; set; }
    public required string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public required string ImageUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public int PoliticalPartyId { get; set; }
}
