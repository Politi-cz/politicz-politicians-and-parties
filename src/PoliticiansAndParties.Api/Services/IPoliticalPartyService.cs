namespace PoliticiansAndParties.Api.Services;

public interface IPoliticalPartyService
{
    /// <summary>
    ///     Creates a political party.
    /// </summary>
    /// <param name="politicalParty">Political party to create.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<PoliticalParty>> CreateAsync(PoliticalParty politicalParty);

    /// <summary>
    ///     Gets a political party.
    /// </summary>
    /// <param name="id">Id of a party.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<PoliticalParty>> GetOneAsync(Guid id);

    /// <summary>
    ///     Gets all political parties.
    /// </summary>
    /// <returns>Result of the operation.</returns>
    Task<Result<IEnumerable<PoliticalParty>>> GetAllAsync();

    /// <summary>
    ///     Updates a political party.
    /// </summary>
    /// <param name="updatePoliticalParty">Updated political party.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<PoliticalParty>> UpdateAsync(UpdatePoliticalPartyDto updatePoliticalParty);

    /// <summary>
    ///     Deletes a political party.
    /// </summary>
    /// <param name="partyId">Id of a party.</param>
    /// <returns>Result of the operation.</returns>
    Task<Result<Guid>> DeleteAsync(Guid partyId);
}
