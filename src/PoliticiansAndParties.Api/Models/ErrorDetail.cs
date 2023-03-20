namespace PoliticiansAndParties.Api.Models;

public record ErrorDetail(string Message, IDictionary<string, string[]>? Errors = null);
