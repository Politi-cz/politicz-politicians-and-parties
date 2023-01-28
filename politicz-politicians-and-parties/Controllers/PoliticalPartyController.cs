using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Services;

namespace politicz_politicians_and_parties.Controllers
{
    [Route("api/political-parties")]
    [ApiController]
    public class PoliticalPartyController : ControllerBase
    {
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IPoliticianService _politicianService;

        public PoliticalPartyController(IPoliticalPartyService politicalPartyService, IPoliticianService politicianService)
        {
            _politicalPartyService = politicalPartyService;
            _politicianService = politicianService;
        }

        [HttpPost("create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePoliticalParty([FromBody] PoliticalPartyCreateDto politicalParty) { 
            var created = await _politicalPartyService.CreatePoliticalParty(politicalParty);

            if (created is false) {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetPoliticalParty), new { id = politicalParty.Id }, politicalParty);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPoliticalParty([FromRoute] Guid id) { 
            var politicalPartyDto = await _politicalPartyService.GetPoliticalPartyAsync(id);

            if (politicalPartyDto is null)
            {
                return NotFound();
            }

            return Ok(politicalPartyDto);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPoliticalParties()
        {
            var politicalPartiesSideNav = await _politicalPartyService.GetPoliticalPartiesAsync();

            return Ok(politicalPartiesSideNav);
        }

        [HttpGet("politician/{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPolitician([FromRoute] Guid id)
        {
            var politicianDto = await _politicianService.GetPoliticianAsync(id);

            if (politicianDto is null)
            {
                return NotFound();
            }

            return Ok(politicianDto);
        }

        [HttpPost("{partyId:guid}/politician")]
        public async Task<IActionResult> CreatePolitician([FromRoute] Guid partyId, [FromBody] PoliticianDto politicianDto) {
            var created = await _politicianService.CreatePoliticianAsync(partyId, politicianDto);

            if (created is false) {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetPolitician), new { id = politicianDto.Id }, politicianDto);
        }
    }
}
