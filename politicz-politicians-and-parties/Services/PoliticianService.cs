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

        public async Task<bool> CreatePoliticianAsync(Guid partyId, PoliticianDto politicianDto)
        {
            politicianDto.Id= Guid.NewGuid();

            _validator.ValidateAndThrow(politicianDto);

            var politician = politicianDto.ToPolitician();

            var internalPartyId = await _politicalPartyRepository.GetPoliticalPartyInternalIdAsync(partyId);

            if (internalPartyId is null) {
                throw new ValidationException($"Political Party with id {partyId} does not exist");
            }

            politician.PoliticalPartyId = (int)internalPartyId;

            return await _politicianRepository.CreatePoliticianAsync(politician);
        }

        public async Task<PoliticianDto?> GetPoliticianAsync(Guid id)
        {

            var politician = await _politicianRepository.GetPoliticianAsync(id);

            return politician?.ToPoliticianDto();
        }
    }
}
