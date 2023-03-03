using politicz_politicians_and_parties.Contracts.Requests;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class ApiToDomainMapper
    {
        public static Politician ToPolitician(this PoliticianRequest politicianRequest) => 
            new()
            {
                FrontEndId = Guid.NewGuid(),
                BirthDate = politicianRequest.BirthDate,
                FacebookUrl = politicianRequest.FacebookUrl,
                InstagramUrl = politicianRequest.InstagramUrl,
                TwitterUrl = politicianRequest.TwitterUrl,
                FullName = politicianRequest.FullName
            };

        public static Politician ToPolitician(this UpdatePoliticianRequest updatePoliticianRequest) =>
            new()
            {
                FrontEndId = updatePoliticianRequest.Id,
                BirthDate = updatePoliticianRequest.Request.BirthDate,
                FacebookUrl = updatePoliticianRequest.Request.FacebookUrl,
                InstagramUrl = updatePoliticianRequest.Request.InstagramUrl,
                TwitterUrl = updatePoliticianRequest.Request.TwitterUrl,
                FullName = updatePoliticianRequest.Request.FullName
            };
    }
}
