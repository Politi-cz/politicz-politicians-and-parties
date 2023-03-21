namespace PoliticiansAndParties.Api.Test.Unit.Services;

public static class TestData
{
    public static Politician GetPolitician => new()
    {
        Id = 1,
        ImageUrl = string.Empty,
        FrontEndId = Guid.NewGuid(),
        BirthDate = DateTime.Now,
        InstagramUrl = "ig.com/pepaKarel",
        FullName = "Pepa Karel",
        PoliticalPartyId = 1,
    };

    public static PoliticalParty GetPoliticalParty => new()
    {
        Id = 1,
        FrontEndId = Guid.NewGuid(),
        ImageUrl = "img.com",
        Name = "Spolu",
        Politicians = new List<Politician> { GetPolitician },
    };

    public static List<PoliticalParty> GetPoliticalParties => new() { GetPoliticalParty };
}
