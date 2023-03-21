namespace PoliticiansAndParties.Api.Results;

/// <summary>
/// Result or NotFound or Failure discriminated union.
/// </summary>
/// <typeparam name="T">Type of Result value.</typeparam>
[GenerateOneOf]
public partial class ResultNotFoundOrFailure<T> : OneOfBase<T, NotFound, Failure>
{
}
