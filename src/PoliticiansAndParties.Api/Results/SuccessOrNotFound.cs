namespace PoliticiansAndParties.Api.Results;

/// <summary>
/// Success or Not Found discriminated union.
/// </summary>
[GenerateOneOf]
public partial class SuccessOrNotFound : OneOfBase<Success, NotFound>
{
}
