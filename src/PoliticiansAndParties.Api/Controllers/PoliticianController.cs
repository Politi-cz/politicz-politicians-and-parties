namespace PoliticiansAndParties.Api.Controllers;

[Route("api/political-parties")]
[ApiController]
public class PoliticianController : ControllerBase
{
    private readonly IPoliticianService _politicianService;
    private readonly IValidator<PoliticianRequest> _validator;

    public PoliticianController(IPoliticianService politicianService, IValidator<PoliticianRequest> validator)
    {
        _politicianService = politicianService;
        _validator = validator;
    }

    [HttpGet("politician/{id:guid}")]
    [ProducesResponseType(200, Type = typeof(PoliticianResponse))]
    public async Task<IActionResult> GetPolitician([FromRoute] Guid id)
    {
        var result = await _politicianService.Get(id);

        return result.Match<IActionResult>(
        success => Ok(success.ToPoliticianResponse()),
        _ => CreateNotFoundResponse(id));
    }

    [HttpPost("{partyId:guid}/politician")]
    [ProducesResponseType(201, Type = typeof(PoliticianResponse))]
    public async Task<IActionResult> CreatePolitician(
        [FromRoute] Guid partyId,
        [FromBody] PoliticianRequest politicianRequest)
    {
        var validationResult = await _validator.ValidateAsync(politicianRequest);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));
        }

        var politician = politicianRequest.ToPolitician();

        var result = await _politicianService.Create(partyId, politician);

        return result.Match<IActionResult>(
            success => CreatedAtAction(nameof(GetPolitician), new { id = success.FrontEndId }, success.ToPoliticianResponse()),
            failure => BadRequest(new ErrorDetail(failure.Message)));
    }

    [HttpPut("politician/{id:guid}")]
    [ProducesResponseType(200, Type = typeof(PoliticianResponse))]
    public async Task<IActionResult> UpdatePolitician([FromRoute] Guid id, [FromBody] PoliticianRequest updatePoliticianRequest)
    {
        var validationResult = await _validator.ValidateAsync(updatePoliticianRequest);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));
        }

        var politician = updatePoliticianRequest.ToPolitician(id);
        var result = await _politicianService.Update(politician);

        return result.Match<IActionResult>(
            success => Ok(success.ToPoliticianResponse()),
            _ => CreateNotFoundResponse(id));
    }

    [HttpDelete("politician/{politicianId:guid}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeletePolitician([FromRoute] Guid politicianId)
    {
        var result = await _politicianService.Delete(politicianId);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => CreateNotFoundResponse(politicianId));
    }

    // TODO: Create a controller class from which will other controllers will inherit and that class will contains these generic responses like this one
    private NotFoundObjectResult CreateNotFoundResponse(Guid entityId) =>
        NotFound(new ErrorDetail($"Politician with id {entityId} not found"));
}
