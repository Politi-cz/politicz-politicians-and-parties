using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class DtoToDomainMappers
    {
        public static PoliticalParty ToPoliticalParty(this PoliticalPartyDto politicalPartyCreateDto)
        {
            return new PoliticalParty
            {
                FrontEndId = politicalPartyCreateDto.Id,
                Name = politicalPartyCreateDto.Name,
                ImageUrl = politicalPartyCreateDto.ImageUrl,
                Tags = politicalPartyCreateDto.Tags,
                Politicians = politicalPartyCreateDto.Politicians.Select(x => x.ToPolitician()).ToList(),
            };
        }

        public static PoliticalParty ToPoliticalParty(this UpdatePoliticalPartyDto update, Guid partyId)
        {
            return new PoliticalParty
            {
                FrontEndId = partyId,
                ImageUrl = update.ImageUrl,
                Name = update.Name,
                Tags = update.Tags
            };
        }

        public static Politician ToPolitician(this PoliticianDto politicianDto)
        {
            return new Politician
            {
                FrontEndId = politicianDto.Id,
                BirthDate = politicianDto.BirthDate,
                FacebookUrl = politicianDto.FacebookUrl,
                InstagramUrl = politicianDto.InstagramUrl,
                TwitterUrl = politicianDto.TwitterUrl,
                FullName = politicianDto.FullName
            };
        }
    }
}
