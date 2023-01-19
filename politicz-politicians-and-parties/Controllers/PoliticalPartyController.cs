using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Services;

namespace politicz_politicians_and_parties.Controllers
{
    [Route("api/politicalParties")]
    [ApiController]
    public class PoliticalPartyController : ControllerBase
    {
        private readonly IPoliticalPartyService _politicalPartyService;

        public PoliticalPartyController(IPoliticalPartyService politicalPartyService)
        {
            _politicalPartyService = politicalPartyService;
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
    }
}
