namespace PoliticiansAndParties.Api.Validators;

public static class HelperValidatorMethods
{
    public static bool IsValidUrl(string? url) => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    public static ValidationFailure[] GenerateValidationError(string paramName, string message) => new[]
        {
            new ValidationFailure(paramName, message),
        };
}
