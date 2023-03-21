namespace PoliticiansAndParties.Api.Results;

/// <summary>
/// Result or NotFound discriminated union.
/// </summary>
/// <typeparam name="T">Type of Result value.</typeparam>
[GenerateOneOf]
public partial class ResultOrNotFound<T> : OneOfBase<T, NotFound>
{
}
