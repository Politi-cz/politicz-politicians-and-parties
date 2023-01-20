using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPoliticalParty([FromRoute] Guid id) { 
            var politicalPartyDto = await _politicalPartyService.GetPoliticalPartyAsync(id);

            if (politicalPartyDto is null)
            {
                return NotFound();
            }

            return Ok(politicalPartyDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetPoliticalParties()
        {
            var politicalPartiesSideNav = await _politicalPartyService.GetPoliticalPartiesAsync();

            return Ok(politicalPartiesSideNav);
        }

        [HttpGet("politician/{id:guid}")]
        public async Task<IActionResult> GetPolitician([FromRoute] Guid id)
        {
            var politicianDto = await _politicianService.GetPoliticianAsync(id);

            if (politicianDto is null)
            {
                return NotFound();
            }

            return Ok(politicianDto);
        }
    }
}
