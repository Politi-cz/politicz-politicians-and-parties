using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Services;

namespace politicz_politicians_and_parties.Controllers
{
    [Route("api/political-parties")]
    [ApiController]
    public class PoliticianController : ControllerBase
    {
        private readonly IPoliticianService _politicianService;

        public PoliticianController(IPoliticianService politicianService)
        {
            _politicianService = politicianService;
        }

        [HttpGet("politician/{id:guid}")]
        [ProducesResponseType(200, Type = typeof(PoliticianDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetPolitician([FromRoute] Guid id)
        {
            var politicianDto = await _politicianService.GetAsync(id);

            if (politicianDto is null)
            {
                return NotFound();
            }

            return Ok(politicianDto);
        }

        [HttpPost("{partyId:guid}/politician")]
        [ProducesResponseType(201, Type = typeof(PoliticianDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetails))]
        [ProducesResponseType(500, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreatePolitician([FromRoute] Guid partyId, [FromBody] PoliticianDto politicianDto)
        {
            var created = await _politicianService.CreateAsync(partyId, politicianDto);

            if (created is false)
            {
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(GetPolitician), new { id = politicianDto.Id }, politicianDto);
        }

        [HttpPut("politician/{politicianId:guid}")]
        [ProducesResponseType(200, Type = typeof(PoliticianDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetails))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetails))]
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
        [ProducesResponseType(500, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeletePolitician([FromRoute] Guid politicianId)
        {
            var deleted = await _politicianService.DeleteAsync(politicianId);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
