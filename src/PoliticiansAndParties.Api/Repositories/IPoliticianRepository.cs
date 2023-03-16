namespace PoliticiansAndParties.Api.Repositories;

public interface IPoliticianRepository
{
    /// <summary>
    /// Gets a <see cref="Politician"/>.
    /// </summary>
    /// <param name="frontEndId">Front-end id of <see cref="Politician"/>.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<Politician>> Get(Guid frontEndId);

    /// <summary>
    /// Creates a <see cref="Politician"/>.
    /// </summary>
    /// <param name="politician"><see cref="Politician"/> to create.</param>
    /// <returns>Created <see cref="Politician"/>.</returns>
    Task<Politician> CreateOne(Politician politician);

    /// <summary>
    /// Creates <see cref="IEnumerable{T}"/> of <see cref="Politician"/>.
    /// </summary>
    /// <param name="politicians">Politicians to create.</param>
    /// <param name="transaction">Transaction.</param>
    /// <returns>True/false if politicians were created.</returns>
    Task<bool> CreateAll(IEnumerable<Politician> politicians, IDbTransaction transaction);

    /// <summary>
    /// Updates a <see cref="Politician"/>.
    /// </summary>
    /// <param name="politician">Updated <see cref="Politician"/>.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<Politician>> Update(Politician politician);

    /// <summary>
    /// Deletes a <see cref="Politician"/>.
    /// </summary>
    /// <param name="frontEndId">Front-end id of <see cref="Politician"/>.</param>
    /// <returns>Discriminated union <see cref="SuccessOrNotFound"/>.</returns>
    Task<SuccessOrNotFound> Delete(Guid frontEndId);
}
