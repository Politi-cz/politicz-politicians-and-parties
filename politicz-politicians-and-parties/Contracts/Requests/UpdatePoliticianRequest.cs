using Microsoft.AspNetCore.Mvc;

namespace politicz_politicians_and_parties.Contracts.Requests;

public record UpdatePoliticianRequest
{
    [FromRoute(Name = "id")] public required Guid Id { get; init; }
    [FromBody] public required PoliticianRequest Request { get; init; }
}