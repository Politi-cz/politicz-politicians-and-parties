using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticianService : IPoliticianService
    {
        private readonly IPoliticianRepository _politicianRepository;
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly IValidator<PoliticianDto> _validator;

        public PoliticianService(IPoliticianRepository politicianRepository, IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticianDto> validator)
        {
            _politicianRepository = politicianRepository;
            _politicalPartyRepository= politicalPartyRepository;
            _validator = validator;
        }

        public async Task<bool> CreateAsync(Guid partyId, PoliticianDto politicianDto)
        {
            _validator.ValidateAndThrow(politicianDto);

            var politician = politicianDto.ToPolitician();

            var internalPartyId = await _politicalPartyRepository.GetInternalIdAsync(partyId);

            if (internalPartyId is null) {
                throw new ValidationException($"Political Party with id {partyId} does not exist");
            }

            politician.PoliticalPartyId = (int)internalPartyId;

            return await _politicianRepository.CreateOneAsync(politician);
        }

        public async Task<PoliticianDto?> GetAsync(Guid id)
        {

            var politician = await _politicianRepository.GetAsync(id);

            return politician?.ToPoliticianDto();
        }
    }
}
