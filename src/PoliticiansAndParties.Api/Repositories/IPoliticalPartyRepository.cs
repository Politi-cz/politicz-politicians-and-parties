namespace PoliticiansAndParties.Api.Repositories;

public interface IPoliticalPartyRepository
{
    /// <summary>
    /// Gets all political parties.
    /// </summary>
    /// <returns>Political parties.</returns>
    Task<IEnumerable<PoliticalParty>> GetAll();

    /// <summary>
    /// Gets a <see cref="PoliticalParty"/>.
    /// </summary>
    /// <param name="frontEndId">Front-end id of a political party.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<PoliticalParty>> GetOne(Guid frontEndId);

    /// <summary>
    /// Gets an internal ID of <see cref="PoliticalParty"/>.
    /// </summary>
    /// <param name="frontEndId">Front-end id of a political party.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<int>> GetInternalId(Guid frontEndId);

    /// <summary>
    /// Creates a <see cref="PoliticalParty"/>.
    /// </summary>
    /// <param name="politicalParty"><see cref="PoliticalParty"/> to create.</param>
    /// <returns>Created <see cref="PoliticalParty"/>.</returns>
    Task<PoliticalParty> Create(PoliticalParty politicalParty);

    /// <summary>
    /// Checks if a <see cref="PoliticalParty"/> exists by name.
    /// </summary>
    /// <param name="partyName">Name of party.</param>
    /// <returns>True/false if a party exists.</returns>
    Task<bool> ExistsByName(string partyName);

    /// <summary>
    /// Updates a <see cref="PoliticalParty"/>.
    /// </summary>
    /// <param name="politicalParty">Updated <see cref="PoliticalParty"/>.</param>
    /// <returns>Discriminated union <see cref="ResultOrNotFound{T}"/>.</returns>
    Task<ResultOrNotFound<PoliticalParty>> Update(PoliticalParty politicalParty);

    /// <summary>
    /// Deletes a <see cref="PoliticalParty"/>.
    /// </summary>
    /// <param name="partyId">Id of a party to delete.</param>
    /// <returns>Discriminated union <see cref="SuccessOrNotFound"/>.</returns>
    Task<SuccessOrNotFound> Delete(Guid partyId);
}
