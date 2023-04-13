#pragma warning disable SA1206
namespace PoliticiansAndParties.Api.Options;

public sealed class PoliticiansPartiesOptions
{
    public required DatabaseOptions Database { get; set; }

    public required Auth0Options Auth0 { get; set; }
}
