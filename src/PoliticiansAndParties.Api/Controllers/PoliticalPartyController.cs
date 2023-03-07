using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PoliticiansAndParties.Api.Contracts.Requests;
using PoliticiansAndParties.Api.Contracts.Responses;
using PoliticiansAndParties.Api.Dtos;
using PoliticiansAndParties.Api.Mapping;
using PoliticiansAndParties.Api.Models;
using PoliticiansAndParties.Api.Result;
using PoliticiansAndParties.Api.Services;

namespace PoliticiansAndParties.Api.Controllers;

[Route("api/political-parties")]
[ApiController]
public class PoliticalPartyController : ControllerBase
{
    private readonly IPoliticalPartyService _politicalPartyService;
    private readonly IValidator<PoliticalPartyRequest> _validator;


    public PoliticalPartyController(IPoliticalPartyService politicalPartyService,
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
            return BadRequest(new ErrorDetail("Validation error", validationResult.ToDictionary()));

        var result = await _politicalPartyService.CreateAsync(politicalPartyRequest.ToPoliticalParty());

        if (result.IsError)
            return HandleErrorResult(result);

        return CreatedAtAction(nameof(GetPoliticalParty), new { id = result.Data!.Id },
            result.Data.ToPoliticalPartyResponse());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(200, Type = typeof(PoliticalPartyResponse))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> GetPoliticalParty([FromRoute] Guid id)
    {
        var result = await _politicalPartyService.GetOneAsync(id);

        if (result.IsError)
            return HandleErrorResult(result);

        return Ok(result.Data!.ToPoliticalPartyResponse());
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<PoliticalPartySideNavDto>))]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> GetPoliticalParties()
    {
        var result = await _politicalPartyService.GetAllAsync();

        if (result.IsError)
            HandleErrorResult(result);

        return Ok(result.Data!.Select(x => x.ToPoliticalPartySideNavDto()));
    }


    [HttpPut("{partyId:guid}")]
    [ProducesResponseType(200, Type = typeof(UpdatePoliticalPartyDto))]
    [ProducesResponseType(400, Type = typeof(ErrorDetail))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> UpdatePoliticalParty([FromRoute] Guid partyId,
        [FromBody] UpdatePoliticalPartyDto updatePoliticalParty)
    {
        updatePoliticalParty.Id = partyId;
        var updated = await _politicalPartyService.UpdateAsync(updatePoliticalParty);

        if (!updated) return NotFound();

        return Ok(updatePoliticalParty);
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

    private IActionResult HandleErrorResult<T>(Result<T> result)
    {
        return result.ErrorType switch
        {
            ErrorType.Invalid => BadRequest(result.Error),
            ErrorType.NotFound => NotFound(result.Error),
            _ => StatusCode(500)
        };
    }
}
