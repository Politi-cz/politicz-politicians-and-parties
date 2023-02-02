using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Models;
using System.Net.NetworkInformation;

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

        public static PoliticalPartySideNavDto ToPoliticalPartySideNavDto(this PoliticalParty politicalParty) {
            return new PoliticalPartySideNavDto
            {
                Id = politicalParty.FrontEndId,
                ImageUrl = politicalParty.ImageUrl,
                Name = politicalParty.Name,
                Tags= politicalParty.Tags
            };
        }

        public static PoliticalPartyDto ToPoliticalPartyDto(this PoliticalParty politicalParty) {
            return new PoliticalPartyDto
            {
                Id = politicalParty.FrontEndId,
                ImageUrl = politicalParty.ImageUrl,
                Name = politicalParty.Name,
                Politicians = politicalParty.Politicians.Select(x => x.ToPoliticianDto()).ToList(),
                Tags = politicalParty.Tags
            };
        }
    }
}
