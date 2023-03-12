namespace PoliticiansAndParties.Api.Mapping;

public static class DomainToApiMapper
{
    public static PoliticianResponse ToPoliticianResponse(this Politician politician) => new(
            politician.FrontEndId,
            politician.FullName,
            politician.BirthDate,
            politician.ImageUrl,
            politician.InstagramUrl,
            politician.TwitterUrl,
            politician.FacebookUrl);

    public static PoliticalPartyResponse ToPoliticalPartyResponse(this PoliticalParty politicalParty) => new()
    {
        Id = politicalParty.FrontEndId,
        Name = politicalParty.Name,
        ImageUrl = politicalParty.ImageUrl,
        Politicians = politicalParty.Politicians.Select(x => x.ToPoliticianResponse()),
        Tags = politicalParty.Tags,
    };

    public static PartySideNavResponse ToPartySideNav(this PoliticalParty politicalParty) => new()
    {
        Id = politicalParty.FrontEndId,
        ImageUrl = politicalParty.ImageUrl,
        Name = politicalParty.Name,
        Tags = politicalParty.Tags,
    };
}
