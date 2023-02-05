using FluentValidation;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Mapping;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Validators;

namespace politicz_politicians_and_parties.Services
{
    public class PoliticianService : IPoliticianService
    {
        readonly IPoliticianRepository _politicianRepository;
        readonly IPoliticalPartyRepository _politicalPartyRepository;
        readonly IValidator<PoliticianDto> _validator;

        public PoliticianService(IPoliticianRepository politicianRepository, IPoliticalPartyRepository politicalPartyRepository, IValidator<PoliticianDto> validator)
        {
            _politicianRepository = politicianRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _validator = validator;
        }

        public async Task<bool> CreateAsync(Guid partyId, PoliticianDto politicianDto)
        {
            _validator.ValidateAndThrow(politicianDto);

            politicianDto.Id = Guid.NewGuid();
            var politician = politicianDto.ToPolitician();

            var internalPartyId = await _politicalPartyRepository.GetInternalIdAsync(partyId);

            if (internalPartyId is null)
            {
                var msg = $"Political party with id {partyId} does not exist";
                throw new ValidationException(msg, HelperValidatorMethods.GenerateValidationError(nameof(politicianDto.Id), msg));
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
