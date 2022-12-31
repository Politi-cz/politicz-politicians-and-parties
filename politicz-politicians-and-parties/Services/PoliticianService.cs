using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticianService : IPoliticianService
    {
        private readonly IPoliticianRepository _politicianRepository;

        public PoliticianService(IPoliticianRepository politicianRepository)
        {
            _politicianRepository = politicianRepository;
        }

        public async Task<PoliticianDto?> GetPoliticianAsync(Guid id)
        {
            Politician? politician;
            try { 
                politician = await _politicianRepository.GetPolitician(id);
            }catch(Exception)
            {
                throw;
            }

            return politician?.ToPoliticianDto();
        }
    }
}
