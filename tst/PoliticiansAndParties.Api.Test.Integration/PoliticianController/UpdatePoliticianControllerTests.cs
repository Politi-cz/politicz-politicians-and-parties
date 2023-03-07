using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace PoliticiansAndParties.Api.Test.Integration.PoliticianController;

public class UpdatePoliticianControllerTests : IClassFixture<PoliticiansAndPartiesApiFactory>
{
    private readonly HttpClient _client;

    public UpdatePoliticianControllerTests(PoliticiansAndPartiesApiFactory apiFactory) =>
        _client = apiFactory.CreateClient();

    [Fact]
    public async Task UpdateAsync_UpdatesPolitician_WhenDataValid()
    {
        // Arrange
        var parentParty = DataGenerator.GeneratePoliticalParty(1);
        var createPartyResponse =
            await _client.PostAsJsonAsync("api/political-parties/create", parentParty);
        var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

        var generatedPolitician = DataGenerator.GeneratePolitician();
        var createPoliticianResponse =
            await _client.PostAsJsonAsync($"api/political-parties/{createdParty!.Id}/politician",
                generatedPolitician);
        var createdPolitician =
            await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();

        var updatedPolitician = DataGenerator.GeneratePolitician();
        updatedPolitician.Id = createdPolitician!.Id;

        // Act
        var updatePoliticianResponse =
            await _client.PutAsJsonAsync(
                $"api/political-parties/politician/{createdPolitician!.Id}",
                updatedPolitician);
        var getPoliticianResponse =
            await _client.GetAsync($"api/political-parties/politician/{createdPolitician!.Id}");

        // Assert
        updatePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await updatePoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();
        result.Should().BeEquivalentTo(updatedPolitician,
            options => options
                .Using<DateTime>(ctx =>
                    ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>());

        getPoliticianResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedPolitician =
            await getPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();
        returnedPolitician.Should().BeEquivalentTo(result,
            options => options
                .Using<DateTime>(ctx =>
                    ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task UpdateAsync_ReturnsErrorDetails_WhenDataInvalid()
    {
        // Arrange
        var parentParty = DataGenerator.GeneratePoliticalParty(1);
        var createPartyResponse =
            await _client.PostAsJsonAsync("api/political-parties/create", parentParty);
        var createdParty = await createPartyResponse.Content.ReadFromJsonAsync<PoliticalPartyDto>();

        var generatedPolitician = DataGenerator.GeneratePolitician();
        var createPoliticianResponse =
            await _client.PostAsJsonAsync($"api/political-parties/{createdParty!.Id}/politician",
                generatedPolitician);
        var createdPolitician =
            await createPoliticianResponse.Content.ReadFromJsonAsync<PoliticianDto>();

        var updatedPolitician = DataGenerator.GeneratePolitician();
        updatedPolitician.Id = createdPolitician!.Id;
        updatedPolitician.InstagramUrl = "jklsdfalkjdsalkj"; // invalid url

        var expectedError = new ErrorDetail("Validation error")
        {
            Errors = new Dictionary<string, string[]>
            {
                { "InstagramUrl", new[] { "Must be a valid url." } }
            }
        };

        // Act
        var updatePoliticianResponse =
            await _client.PutAsJsonAsync(
                $"api/political-parties/politician/{createdPolitician!.Id}",
                updatedPolitician);

        // Assert
        updatePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await updatePoliticianResponse.Content.ReadFromJsonAsync<ErrorDetail>();
        result.Should().BeEquivalentTo(expectedError);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenPoliticianNotFound()
    {
        // Arrange
        var updatedPolitician = DataGenerator.GeneratePolitician();
        var nonExistingId = Guid.NewGuid();

        // Act
        var updatePoliticianResponse =
            await _client.PutAsJsonAsync($"api/political-parties/politician/{nonExistingId}",
                updatedPolitician);

        // Assert
        updatePoliticianResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
