namespace PoliticiansAndParties.Api.Contracts.Requests;

public record PoliticalPartyRequest(
    string Name,
    string ImageUrl,
    HashSet<string> Tags,
    List<PoliticianRequest> Politicians);
