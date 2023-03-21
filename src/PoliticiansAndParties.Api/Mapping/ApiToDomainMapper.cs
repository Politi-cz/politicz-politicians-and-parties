namespace PoliticiansAndParties.Api.Mapping;

public static class ApiToDomainMapper
{
    public static Politician ToPolitician(this PoliticianRequest politicianRequest) => new()
    {
        FrontEndId = Guid.NewGuid(),
        BirthDate = politicianRequest.BirthDate,
        FacebookUrl = politicianRequest.FacebookUrl,
        ImageUrl = politicianRequest.ImageUrl,
        InstagramUrl = politicianRequest.InstagramUrl,
        TwitterUrl = politicianRequest.TwitterUrl,
        FullName = politicianRequest.FullName,
    };

    public static Politician ToPolitician(this PoliticianRequest politicianRequest, Guid id) => new()
    {
        FrontEndId = id,
        BirthDate = politicianRequest.BirthDate,
        FacebookUrl = politicianRequest.FacebookUrl,
        ImageUrl = politicianRequest.ImageUrl,
        InstagramUrl = politicianRequest.InstagramUrl,
        TwitterUrl = politicianRequest.TwitterUrl,
        FullName = politicianRequest.FullName,
    };

    public static PoliticalParty ToPoliticalParty(this PoliticalPartyRequest politicalPartyRequest) => new()
    {
        FrontEndId = Guid.NewGuid(),
        Name = politicalPartyRequest.Name,
        ImageUrl = politicalPartyRequest.ImageUrl,
        Politicians = politicalPartyRequest.Politicians.Select(x => x.ToPolitician()).ToList(),
        Tags = politicalPartyRequest.Tags,
    };

    public static PoliticalParty ToPoliticalParty(this PoliticalPartyRequest politicalPartyRequest, Guid partyId) => new()
    {
        FrontEndId = partyId,
        Name = politicalPartyRequest.Name,
        ImageUrl = politicalPartyRequest.ImageUrl,
        Politicians = politicalPartyRequest.Politicians.Select(x => x.ToPolitician()).ToList(),
        Tags = politicalPartyRequest.Tags,
    };
}
