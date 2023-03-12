namespace PoliticiansAndParties.Api.Repositories;

public interface IPoliticianRepository
{
    Task<Politician?> GetAsync(Guid frontEndId);

    Task<Politician> CreateOneAsync(Politician politician);

    Task<bool> CreateAllAsync(IEnumerable<Politician> politicians, IDbTransaction transaction);

    Task<bool> UpdateAsync(Politician politician);

    Task<bool> DeleteAsync(Guid frontEndId);
}
