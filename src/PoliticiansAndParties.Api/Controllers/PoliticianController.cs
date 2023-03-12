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
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> GetPolitician([FromRoute] Guid id)
    {
        var result = await _politicianService.GetAsync(id);

        return result.IsError ? HandleErrorResult(result) : Ok(result.Data!.ToPoliticianResponse());
    }

    [HttpPost("{partyId:guid}/politician")]
    [ProducesResponseType(201, Type = typeof(PoliticianResponse))]
    [ProducesResponseType(400, Type = typeof(ErrorDetail))]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
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

        var result = await _politicianService.CreateAsync(partyId, politician);

        if (result.IsError)
        {
            return HandleErrorResult(result);
        }

        var response = result.Data!.ToPoliticianResponse();

        return CreatedAtAction(nameof(GetPolitician), new { id = response.Id }, response);
    }

    [HttpPut("politician/{id:guid}")]
    [ProducesResponseType(200, Type = typeof(PoliticianResponse))]
    [ProducesResponseType(400, Type = typeof(ErrorDetail))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> UpdatePolitician(
        [FromRoute][FromBody] UpdatePoliticianRequest updatePoliticianRequest)
    {
        var validationResult = await _validator.ValidateAsync(updatePoliticianRequest.Request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));
        }

        var politician = updatePoliticianRequest.ToPolitician();
        var result = await _politicianService.UpdateAsync(politician);

        return result.IsError ? HandleErrorResult(result) : Ok(politician.ToPoliticianResponse());
    }

    [HttpDelete("politician/{politicianId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> DeletePolitician([FromRoute] Guid politicianId)
    {
        var result = await _politicianService.DeleteAsync(politicianId);

        return result.IsError ? HandleErrorResult(result) : Ok();
    }

    private IActionResult HandleErrorResult<T>(Result<T> result) => result.ErrorType switch
    {
        ErrorType.Invalid => BadRequest(result.Error),
        ErrorType.NotFound => NotFound(result.Error),
        ErrorType.None => throw new NotImplementedException(),
        ErrorType.InternalError => throw new NotImplementedException(),
        _ => StatusCode(500),
    };
}
