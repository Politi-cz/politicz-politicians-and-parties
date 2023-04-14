#pragma warning disable SA1206
namespace PoliticiansAndParties.Api.Options;

public class Auth0Options
{
    public required string Domain { get; set; }

    public required string Audience { get; set; }
}
