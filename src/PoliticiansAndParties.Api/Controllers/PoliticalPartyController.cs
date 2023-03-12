namespace PoliticiansAndParties.Api.Controllers;

[Route("api/political-parties")]
[ApiController]
public class PoliticalPartyController : ControllerBase
{
    private readonly IPoliticalPartyService _politicalPartyService;
    private readonly IValidator<PoliticalPartyRequest> _validator;

    public PoliticalPartyController(
        IPoliticalPartyService politicalPartyService,
        IValidator<PoliticalPartyRequest> validator)
    {
        _politicalPartyService = politicalPartyService;
        _validator = validator;
    }

    [HttpPost("create")]
    [ProducesResponseType(201, Type = typeof(PoliticalPartyResponse))]
    [ProducesResponseType(400, Type = typeof(ErrorDetail))]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> CreatePoliticalParty([FromBody] PoliticalPartyRequest politicalPartyRequest)
    {
        var validationResult = await _validator.ValidateAsync(politicalPartyRequest);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));
        }

        var result = await _politicalPartyService.CreateAsync(politicalPartyRequest.ToPoliticalParty());

        return result.IsError
            ? HandleErrorResult(result)
            : CreatedAtAction(nameof(GetPoliticalParty), new { id = result.Data!.Id }, result.Data.ToPoliticalPartyResponse());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(PoliticalPartyResponse))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> GetPoliticalParty([FromRoute] Guid id)
    {
        var result = await _politicalPartyService.GetOneAsync(id);

        return result.IsError ? HandleErrorResult(result) : Ok(result.Data!.ToPoliticalPartyResponse());
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<PoliticalPartySideNavDto>))]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> GetPoliticalParties()
    {
        var result = await _politicalPartyService.GetAllAsync();

        if (result.IsError)
        {
            _ = HandleErrorResult(result);
        }

        return Ok(result.Data!.Select(x => x.ToPoliticalPartySideNavDto()));
    }

    [HttpPut("{partyId:guid}")]
    [ProducesResponseType(200, Type = typeof(UpdatePoliticalPartyDto))]
    [ProducesResponseType(400, Type = typeof(ErrorDetail))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> UpdatePoliticalParty(
        [FromRoute] Guid partyId,
        [FromBody] UpdatePoliticalPartyDto updatePoliticalParty)
    {
        updatePoliticalParty.Id = partyId;
        var updated = await _politicalPartyService.UpdateAsync(updatePoliticalParty);

        return !updated ? NotFound() : (IActionResult)Ok(updatePoliticalParty);
    }

    [HttpDelete("{partyId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> DeletePoliticalParty([FromRoute] Guid partyId)
    {
        var result = await _politicalPartyService.DeleteAsync(partyId);

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
