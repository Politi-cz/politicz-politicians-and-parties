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

    public static PoliticalPartyResponse ToPoliticalPartyResponse(this PoliticalParty politicalParty) => new(
            politicalParty.FrontEndId,
            politicalParty.Name,
            politicalParty.ImageUrl,
            politicalParty.Tags,
            politicalParty.Politicians.Select(x => x.ToPoliticianResponse()));

    public static PartySideNavResponse ToPartySideNav(this PoliticalParty politicalParty) => new(
        politicalParty.FrontEndId,
        politicalParty.Name,
        politicalParty.ImageUrl,
        politicalParty.Tags);
}
