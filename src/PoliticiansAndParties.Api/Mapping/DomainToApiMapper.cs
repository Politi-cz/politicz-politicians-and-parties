using PoliticiansAndParties.Api.Contracts.Responses;
using PoliticiansAndParties.Api.Models;

namespace PoliticiansAndParties.Api.Mapping;

public static class DomainToApiMapper
{
    public static PoliticianResponse ToPoliticianResponse(this Politician politician)
    {
        return new PoliticianResponse(
            politician.FrontEndId,
            politician.FullName,
            politician.BirthDate,
            politician.ImageUrl,
            politician.InstagramUrl,
            politician.TwitterUrl,
            politician.FacebookUrl
        );
    }

    public static PoliticalPartyResponse ToPoliticalPartyResponse(this PoliticalParty politicalParty)
    {
        return new PoliticalPartyResponse
        {
            Id = politicalParty.FrontEndId,
            Name = politicalParty.Name,
            ImageUrl = politicalParty.ImageUrl,
            Politicians = politicalParty.Politicians.Select(x => x.ToPoliticianResponse()),
            Tags = politicalParty.Tags
        };
    }

    public static PartySideNavResponse ToPartySideNav(this PoliticalParty politicalParty)
    {
        return new PartySideNavResponse
        {
            Id = politicalParty.FrontEndId,
            ImageUrl = politicalParty.ImageUrl,
            Name = politicalParty.Name,
            Tags = politicalParty.Tags
        };
    }
}
