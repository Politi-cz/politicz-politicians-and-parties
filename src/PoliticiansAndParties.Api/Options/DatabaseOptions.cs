#pragma warning disable SA1206
namespace PoliticiansAndParties.Api.Options;

public sealed class DatabaseOptions
{
    public required string Name { get; set; }

    public required string DefaultConnection { get; set; }

    public required string MasterConnection { get; set; }
}
