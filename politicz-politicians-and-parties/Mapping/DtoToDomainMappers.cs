using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class DtoToDomainMappers
    {
        public static PoliticalParty ToPoliticalParty(this PoliticalPartyCreateDto politicalPartyCreateDto) {
            return new PoliticalParty
            {
                FrontEndId = politicalPartyCreateDto.Id,
                Name = politicalPartyCreateDto.Name,
                ImageUrl = politicalPartyCreateDto.ImageUrl,
                Tags = politicalPartyCreateDto.Tags.ToList(),
            };
        }

        public static Politician ToPolitician(this PoliticianDto politicianDto) {
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
