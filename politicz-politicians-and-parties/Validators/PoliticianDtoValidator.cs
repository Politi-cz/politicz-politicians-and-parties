using FluentValidation;
using politicz_politicians_and_parties.Dtos;

namespace politicz_politicians_and_parties.Validators
{
    public class PoliticianDtoValidator : AbstractValidator<PoliticianDto>
    {
        public PoliticianDtoValidator() {
            RuleFor(politician => politician.FullName).NotEmpty().MaximumLength(255);
            RuleFor(politician => politician.BirthDate).NotEmpty();
            RuleFor(politician => politician.TwitterUrl).Must(IsValidUrl).When(politician => politician.TwitterUrl is not null);
            RuleFor(politician => politician.FacebookUrl).Must(IsValidUrl).When(politician => politician.TwitterUrl is not null);
            RuleFor(politician => politician.InstagramUrl).Must(IsValidUrl).When(politician => politician.TwitterUrl is not null);
        }

        bool IsValidUrl(string? url) {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
