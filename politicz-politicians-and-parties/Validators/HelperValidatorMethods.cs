using FluentValidation.Results;

namespace politicz_politicians_and_parties.Validators
{
    public static class HelperValidatorMethods
    {
        public static bool IsValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static ValidationFailure[] GenerateValidationError(string paramName, string message)
        {
            return new[]
            {
            new ValidationFailure(paramName, message)
        };
        }
    }
}
