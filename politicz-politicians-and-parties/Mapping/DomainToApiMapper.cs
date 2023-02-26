using politicz_politicians_and_parties.Contracts.Responses;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class DomainToApiMapper
    {
        public static PoliticianResponse ToPoliticianResponse(this Politician politician)
        {
            return new PoliticianResponse
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
