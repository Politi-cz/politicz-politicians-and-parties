namespace PoliticiansAndParties.Api.Services;

public interface IPoliticianService
{
    /// <summary>
    ///     Gets a politician.
    /// </summary>
    /// <param name="id">Id of politician.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<Politician>> Get(Guid id);

    /// <summary>
    ///     Creates a new politician.
    /// </summary>
    /// <param name="partyId">Id of political party where politician belongs.</param>
    /// <param name="politician">Politician to create.</param>
    /// <returns>Discriminated union <see cref="ResultOrFailure{T}"/>.</returns>
    Task<ResultOrFailure<Politician>> Create(Guid partyId, Politician politician);

    /// <summary>
    ///     Updates a politician.
    /// </summary>
    /// <param name="politician">New politician data.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<Politician>> Update(Politician politician);

    /// <summary>
    ///     Deletes a politician.
    /// </summary>
    /// <param name="frontEndId">Id of a politician.</param>
    /// <returns>Discriminated union <see cref="SuccessOrNotFound"/>.</returns>
    Task<SuccessOrNotFound> Delete(Guid frontEndId);
}
