using Microsoft.AspNetCore.Mvc;

namespace PoliticiansAndParties.Api.Contracts.Requests;

public record UpdatePoliticianRequest(
    [FromRoute(Name = "id")] Guid Id,
    [FromBody] PoliticianRequest Request);
