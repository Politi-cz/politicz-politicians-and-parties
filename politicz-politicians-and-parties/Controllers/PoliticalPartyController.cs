using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Services;

namespace politicz_politicians_and_parties.Controllers
{
    [Route("api/political-parties")]
    [ApiController]
    public class PoliticalPartyController : ControllerBase
    {
        private readonly IPoliticalPartyService _politicalPartyService;

        public PoliticalPartyController(IPoliticalPartyService politicalPartyService)
        {
            _politicalPartyService = politicalPartyService;
        }

        [HttpPost("create")]
        [ProducesResponseType(201, Type = typeof(PoliticalPartyDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetail))]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> CreatePoliticalParty([FromBody] PoliticalPartyDto politicalParty)
        {
            var created = await _politicalPartyService.CreateAsync(politicalParty);

            if (created is false)
            {
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(GetPoliticalParty), new { id = politicalParty.Id }, politicalParty);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(200, Type = typeof(PoliticalPartyDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> GetPoliticalParty([FromRoute] Guid id)
        {
            var politicalPartyDto = await _politicalPartyService.GetOneAsync(id);

            if (politicalPartyDto is null)
            {
                return NotFound();
            }

            return Ok(politicalPartyDto);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PoliticalPartySideNavDto>))]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> GetPoliticalParties()
        {
            var politicalPartiesSideNav = await _politicalPartyService.GetAllAsync();

            return Ok(politicalPartiesSideNav);
        }



        [HttpPut("{partyId:guid}")]
        [ProducesResponseType(200, Type = typeof(UpdatePoliticalPartyDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetail))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> UpdatePoliticalParty([FromRoute] Guid partyId, [FromBody] UpdatePoliticalPartyDto updatePoliticalParty)
        {
            updatePoliticalParty.Id = partyId;
            var updated = await _politicalPartyService.UpdateAsync(updatePoliticalParty);

            if (!updated)
            {
                return NotFound();
            }

            return Ok(updatePoliticalParty);
        }


        [HttpDelete("{partyId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorDetail))]
        public async Task<IActionResult> DeletePoliticalParty([FromRoute] Guid partyId)
        {
            var deleted = await _politicalPartyService.DeleteAsync(partyId);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
