namespace PoliticiansAndParties.Api.Test.Integration;

public static class Helpers
{
    public static async Task<PoliticalPartyResponse> CreatePoliticalParty(HttpClient client)
    {
        var parentParty = TestData.PartyGenerator.Generate();
        var createPartyResponse = await client.PostAsJsonAsync("api/political-parties/create", parentParty);
        var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyResponse>();

        return createdParty!;
    }

    public static async Task<PoliticianResponse> CreatePoliticianWithParty(HttpClient client)
    {
        var parentParty = await CreatePoliticalParty(client);
        var generatedPolitician = TestData.PoliticianGenerator.Generate();
        var createPoliticianResponse = await client.PostAsJsonAsync(
                $"api/political-parties/{parentParty.Id}/politician",
                generatedPolitician);
        var createdPolitician = await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianResponse>();

        return createdPolitician!;
    }

    public static EquivalencyAssertionOptions<T> GetDateTimeConfig<T>(EquivalencyAssertionOptions<T> options) => options.Using<DateTime>(ctx => ctx.Subject.Should()
            .BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
        .WhenTypeIs<DateTime>();
}
