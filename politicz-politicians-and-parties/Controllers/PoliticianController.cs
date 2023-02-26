using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Contracts.Requests;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Result;
using politicz_politicians_and_parties.Services;

namespace politicz_politicians_and_parties.Controllers
{
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
        [ProducesResponseType(200, Type = typeof(PoliticianDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> GetPolitician([FromRoute] Guid id)
        {
            var result = await _politicianService.GetAsync(id);

            if (result.IsError)
            {
                return HandleErrorResult(result);
            }

            return Ok(result.Data!.ToPoliticianResponse());
        }

        [HttpPost("{partyId:guid}/politician")]
        [ProducesResponseType(201, Type = typeof(PoliticianDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetail))]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> CreatePolitician([FromRoute] Guid partyId, [FromBody] PoliticianRequest politicianRequest)
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

        [HttpPut("politician/{politicianId:guid}")]
        [ProducesResponseType(200, Type = typeof(PoliticianDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetail))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> UpdatePolitician([FromRoute] Guid politicianId, [FromBody] PoliticianDto politicianDto)
        {
            var updated = await _politicianService.UpdateAsync(politicianId, politicianDto);

            if (!updated)
            {
                return NotFound();
            }

            return Ok(politicianDto);
        }

        [HttpDelete("politician/{politicianId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> DeletePolitician([FromRoute] Guid politicianId)
        {
            var deleted = await _politicianService.DeleteAsync(politicianId);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }

        private IActionResult HandleErrorResult<T>(Result<T> result)
        {
            switch (result.ErrorType)
            {
                case ErrorType.Invalid:
                    return BadRequest(result?.Error);
                case ErrorType.NotFound:
                    return NotFound();
                default:
                    return StatusCode(500);
            }
        }
    }
}
