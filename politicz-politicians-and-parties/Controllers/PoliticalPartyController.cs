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
        private readonly IPoliticianService _politicianService;

        public PoliticalPartyController(IPoliticalPartyService politicalPartyService, IPoliticianService politicianService)
        {
            _politicalPartyService = politicalPartyService;
            _politicianService = politicianService;
        }

        [HttpPost("create")]
        [ProducesResponseType(201, Type = typeof(PoliticalPartyDto))]
        [ProducesResponseType(400, Type = typeof(ErrorDetails))]
        [ProducesResponseType(500)]
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
        [ProducesResponseType(500)]
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
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPoliticalParties()
        {
            var politicalPartiesSideNav = await _politicalPartyService.GetAllAsync();

            return Ok(politicalPartiesSideNav);
        }

        [HttpGet("politician/{id:guid}")]
        [ProducesResponseType(200, Type = typeof(PoliticianDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePolitician([FromRoute] Guid partyId, [FromBody] PoliticianDto politicianDto)
        {
            var created = await _politicianService.CreateAsync(partyId, politicianDto);

            if (created is false)
            {
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(GetPolitician), new { id = politicianDto.Id }, politicianDto);
        }
    }
}
