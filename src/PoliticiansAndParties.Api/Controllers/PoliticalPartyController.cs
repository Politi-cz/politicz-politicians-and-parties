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
    [Authorize(AuthConstants.ModifyPolicy)]
    [ProducesResponseType(201, Type = typeof(PoliticalPartyResponse))]
    public async Task<IActionResult> CreatePoliticalParty([FromBody] PoliticalPartyRequest politicalPartyRequest)
    {
        var validationResult = await _validator.ValidateAsync(politicalPartyRequest, o => o.IncludeAllRuleSets());

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));
        }

        var result = await _politicalPartyService.Create(politicalPartyRequest.ToPoliticalParty());

        return result.Match<IActionResult>(
            party => CreatedAtAction(nameof(GetPoliticalParty), new { id = party.FrontEndId }, party.ToPoliticalPartyResponse()),
            failure => BadRequest(new ErrorDetail(failure.Message)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(PoliticalPartyResponse))]
    public async Task<IActionResult> GetPoliticalParty([FromRoute] Guid id)
    {
        var result = await _politicalPartyService.GetOne(id);

        return result.Match<IActionResult>(
            success => Ok(success.ToPoliticalPartyResponse()),
            _ => CreateNotFoundResponse(id));
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<PartySideNavResponse>))]
    public async Task<IActionResult> GetPoliticalParties()
    {
        var parties = await _politicalPartyService.GetAll();

        return Ok(parties.Select(x => x.ToPartySideNav()));
    }

    [HttpPut("{partyId:guid}")]
    [Authorize(AuthConstants.ModifyPolicy)]
    [ProducesResponseType(200, Type = typeof(PoliticalPartyRequest))]
    public async Task<IActionResult> UpdatePoliticalParty(
        [FromRoute] Guid partyId, [FromBody] PoliticalPartyRequest politicalPartyRequest)
    {
        var validationResult = await _validator.ValidateAsync(politicalPartyRequest, o => o.IncludeRuleSets("UpdateFields"));

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));
        }

        var partyUpdate = politicalPartyRequest.ToPoliticalParty(partyId);

        var result = await _politicalPartyService.Update(partyUpdate);

        return result.Match<IActionResult>(
            party => Ok(party.ToPoliticalPartyResponse()),
            _ => CreateNotFoundResponse(partyId),
            failure => BadRequest(new ErrorDetail(failure.Message)));
    }

    [HttpDelete("{partyId:guid}")]
    [Authorize(AuthConstants.ModifyPolicy)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeletePoliticalParty([FromRoute] Guid partyId)
    {
        var result = await _politicalPartyService.Delete(partyId);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => CreateNotFoundResponse(partyId));
    }

    // TODO: Create a controller class from which will other controllers will inherit and that class will contains these generic responses like this one
    private NotFoundObjectResult CreateNotFoundResponse(Guid entityId) =>
        NotFound(new ErrorDetail($"Political party with id {entityId} not found"));
}
