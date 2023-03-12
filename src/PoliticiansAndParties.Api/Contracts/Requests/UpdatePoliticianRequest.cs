namespace PoliticiansAndParties.Api.Contracts.Requests;

// TODO: Probably does not work and would have to write a custom binder, figure out something else
public record UpdatePoliticianRequest(
    [FromRoute(Name = "id")] Guid Id,
    [FromBody] PoliticianRequest Request);
