namespace PoliticiansAndParties.Api.Test.Integration;

public static class TestData
{
    public static readonly Faker<PoliticianRequest> PoliticianGenerator = new Faker<PoliticianRequest>()
        .CustomInstantiator(f => new PoliticianRequest(
            f.Person.FullName,
            f.Person.DateOfBirth,
            f.Image.PicsumUrl(),
            "https://randomfburl.com",
            "https://randomfburl.com"));

    public static readonly Faker<PoliticalPartyRequest> PartyGenerator =
        new Faker<PoliticalPartyRequest>()
            .CustomInstantiator(f => new PoliticalPartyRequest(
                f.Company.CompanyName(),
                f.Image.PicsumUrl(),
                f.Lorem.Words(new Random().Next(1, 10)).ToHashSet(),
                PoliticianGenerator.GenerateBetween(2, 10)));

    public static PoliticalPartyRequest GetInvalidParty => new(
        "Test",
        "fasdasd",
        new(),
        new List<PoliticianRequest>());
}
