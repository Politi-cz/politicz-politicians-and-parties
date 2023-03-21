namespace PoliticiansAndParties.Api.Validators;

public static class HelperValidatorMethods
{
    public static bool IsValidUrl(string? url) => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
