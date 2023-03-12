namespace PoliticiansAndParties.Api.Services;

public interface IPoliticianService
{
    /// <summary>
    ///     Gets a politician.
    /// </summary>
    /// <param name="id">Id of politician.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<Politician>> GetAsync(Guid id);

    /// <summary>
    ///     Creates a new politician.
    /// </summary>
    /// <param name="partyId">Id of political party where politician belongs.</param>
    /// <param name="politician">Politician to create.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<Politician>> CreateAsync(Guid partyId, Politician politician);

    /// <summary>
    ///     Updates a politician.
    /// </summary>
    /// <param name="politician">New politician data.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<Politician>> UpdateAsync(Politician politician);

    /// <summary>
    ///     Deletes a politician.
    /// </summary>
    /// <param name="frontEndId">Id of a politician.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<Guid>> DeleteAsync(Guid frontEndId);
}
