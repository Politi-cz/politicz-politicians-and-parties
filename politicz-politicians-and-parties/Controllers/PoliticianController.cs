using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Services;

namespace politicz_politicians_and_parties.Controllers
{
    // TODO ADD DOCUMENTATION
    [Route("api/politicians")]
    [ApiController]
    public class PoliticianController : ControllerBase
    {
        private readonly IPoliticianService _politicianService;

        public PoliticianController(IPoliticianService politicianService)
        {
            _politicianService = politicianService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPolitician([FromRoute] Guid id) { 
            var politicianDto = await _politicianService.GetPoliticianAsync(id);

            if (politicianDto is null) { 
                return NotFound();
            }

            return Ok(politicianDto);
        }
    }
}
