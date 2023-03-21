namespace PoliticiansAndParties.Api.Models;

public class Politician
{
    public int Id { get; set; }

    public Guid FrontEndId { get; set; }

#pragma warning disable SA1206 // Declaration keywords should follow order
    public required string FullName { get; set; }
#pragma warning restore SA1206 // Declaration keywords should follow order

    public DateTime BirthDate { get; set; }

#pragma warning disable SA1206 // Declaration keywords should follow order
    public required string ImageUrl { get; set; }
#pragma warning restore SA1206 // Declaration keywords should follow order

    public string? InstagramUrl { get; set; }

    public string? TwitterUrl { get; set; }

    public string? FacebookUrl { get; set; }

    public int PoliticalPartyId { get; set; }
}
