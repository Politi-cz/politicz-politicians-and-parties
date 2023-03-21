namespace PoliticiansAndParties.Api.Results;

/// <summary>
/// Result or Failure discriminated union.
/// </summary>
/// <typeparam name="T">Type of Result value.</typeparam>
[GenerateOneOf]
public partial class ResultOrFailure<T> : OneOfBase<T, Failure>
{
}
