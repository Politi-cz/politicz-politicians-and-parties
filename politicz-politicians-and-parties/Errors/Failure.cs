namespace PoliticiansAndParties.Api.Errors;

/// <summary>
/// Failure related to a business validation failure.
/// </summary>
/// <param name="Message">Failure's description.</param>
public record Failure(string Message);
