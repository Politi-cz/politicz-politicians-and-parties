namespace PoliticiansAndParties.Api.Services;

public interface IPoliticalPartyService
{
    /// <summary>
    ///     Creates a political party.
    /// </summary>
    /// <param name="politicalParty">Political party to create.</param>
    /// <returns>Discriminated union <see cref="ResultOrFailure{T}"/>.</returns>
    Task<ResultOrFailure<PoliticalParty>> Create(PoliticalParty politicalParty);

    /// <summary>
    ///     Gets a political party.
    /// </summary>
    /// <param name="id">Id of a party.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<PoliticalParty>> GetOne(Guid id);

    /// <summary>
    ///     Gets all political parties.
    /// </summary>
    /// <returns>Political parties.</returns>
    Task<IEnumerable<PoliticalParty>> GetAll();

    /// <summary>
    ///     Updates a political party.
    /// </summary>
    /// <param name="politicalParty">Updated political party.</param>
    /// <returns>Discriminated union <see cref="ResultNotFoundOrFailure{T}"/>.</returns>
    Task<ResultNotFoundOrFailure<PoliticalParty>> Update(PoliticalParty politicalParty);

    /// <summary>
    ///     Deletes a political party.
    /// </summary>
    /// <param name="partyId">Id of a party.</param>
    /// <returns>Discriminated union <see cref="SuccessOrNotFound"/>.</returns>
    Task<SuccessOrNotFound> Delete(Guid partyId);
}
