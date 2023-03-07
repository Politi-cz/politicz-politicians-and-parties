using PoliticiansAndParties.Api.Dtos;
using PoliticiansAndParties.Api.Models;

namespace PoliticiansAndParties.Api.Mapping;

public static class DomainToDtoMapper
{
    public static PoliticianDto ToPoliticianDto(this Politician politician)
    {
        return new PoliticianDto
        {
            Id = politician.FrontEndId,
            FullName = politician.FullName,
            BirthDate = politician.BirthDate,
            FacebookUrl = politician.FacebookUrl,
            InstagramUrl = politician.InstagramUrl,
            TwitterUrl = politician.TwitterUrl
        };
    }

    public static PoliticalPartySideNavDto ToPoliticalPartySideNavDto(this PoliticalParty politicalParty)
    {
        return new PoliticalPartySideNavDto
        {
            Id = politicalParty.FrontEndId,
            ImageUrl = politicalParty.ImageUrl,
            Name = politicalParty.Name,
            Tags = politicalParty.Tags
        };
    }

    public static PoliticalPartyDto ToPoliticalPartyDto(this PoliticalParty politicalParty)
    {
        return new PoliticalPartyDto
        {
            Id = politicalParty.FrontEndId,
            ImageUrl = politicalParty.ImageUrl,
            Name = politicalParty.Name,
            Politicians = politicalParty.Politicians.Select(x => x.ToPoliticianDto()).ToList(),
            Tags = politicalParty.Tags
        };
    }
}
