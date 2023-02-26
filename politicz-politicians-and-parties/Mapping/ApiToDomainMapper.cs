using politicz_politicians_and_parties.Contracts.Requests;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class ApiToDomainMapper
    {
        public static Politician ToPolitician(this PoliticianRequest politicianRequest)
        {
            return new Politician
            {
                FrontEndId = Guid.NewGuid(),
                BirthDate = politicianRequest.BirthDate,
                FacebookUrl = politicianRequest.FacebookUrl,
                InstagramUrl = politicianRequest.InstagramUrl,
                TwitterUrl = politicianRequest.TwitterUrl,
                FullName = politicianRequest.FullName
            };
        }
    }
}
