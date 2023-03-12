namespace PoliticiansAndParties.Api.Test.Integration;

public static class DataGenerator
{
    private static readonly Faker<PoliticalPartyDto> _partyGenerator =
        new Faker<PoliticalPartyDto>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(x => x.Tags, f => f.Lorem.Words(new Random().Next(1, 10)).ToHashSet())
            .RuleFor(x => x.Id, Guid.NewGuid());

    // TODO: Change to PoliticianRequest
    private static readonly Faker<PoliticianDto> _politicianGenerator = new Faker<PoliticianDto>()
        .RuleFor(x => x.FullName, f => f.Person.FullName)
        .RuleFor(x => x.BirthDate, f => f.Person.DateOfBirth)
        .RuleFor(x => x.FacebookUrl, "https://randomfburl.com")
        .RuleFor(x => x.InstagramUrl, "https://randomigurl.com")
        .RuleFor(x => x.TwitterUrl, "https://randomtwurl.com")
        .RuleFor(x => x.Id, Guid.NewGuid());

    public static IEnumerable<PoliticalPartyDto> GeneratePoliticalParties(
        int count = 10,
        int maxPoliticiansInParty = 20)
    {
        var parties = _partyGenerator.Generate(count);

        parties.ForEach(x =>
            x.Politicians = _politicianGenerator.GenerateBetween(1, maxPoliticiansInParty));

        return parties;
    }

    public static PoliticalPartyDto GeneratePoliticalParty(int maxPoliticians = 20)
    {
        var party = _partyGenerator.Generate();
        party.Politicians = _politicianGenerator.GenerateBetween(1, maxPoliticians);

        return party;
    }

    public static PoliticianDto GeneratePolitician() => _politicianGenerator.Generate();
}
