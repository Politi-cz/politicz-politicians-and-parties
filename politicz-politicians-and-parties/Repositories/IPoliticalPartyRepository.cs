﻿using politicz_politicians_and_parties.Models;

namespace politicz_politicians_and_parties.Repositories
{
    public interface IPoliticalPartyRepository
    {
        Task<IEnumerable<PoliticalParty>> GetPoliticalPartiesAsync();
        Task<PoliticalParty?> GetPoliticalPartyAsync(Guid frontEndId);
    }
}
