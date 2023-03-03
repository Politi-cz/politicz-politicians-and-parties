using politicz_politicians_and_parties.Contracts.Responses;
using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Mapping
{
    public static class DomainToApiMapper
    {
        public static PoliticianResponse ToPoliticianResponse(this Politician politician) =>
            new(politician.FrontEndId,
                politician.FullName,
                politician.BirthDate,
                politician.InstagramUrl,
                politician.TwitterUrl,
                politician.FacebookUrl
            );
    }
}
