using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class DomainToDtoMapper
    {
        public static PoliticianDto ToPoliticianDto(this Politician politician) {
            return new PoliticianDto
            {
                Id = politician.FrontEndId,
                FullName = politician.FullName,
                BirthDate = politician.BirthDate,
                FacebookUrl = politician.FacebookUrl,
                InstagramUrl = politician.InstagramUrl,
                TwitterUrl = politician.TwitterUrl,
            };
        }
    }
}
