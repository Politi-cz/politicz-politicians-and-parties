using FluentValidation;
using politicz_politicians_and_parties.Contracts.Requests;

namespace politicz_politicians_and_parties.Validators
{
    public class PoliticianRequestValidator : AbstractValidator<PoliticianRequest>
    {
        public PoliticianRequestValidator()
        {
            RuleFor(politician => politician.FullName).NotEmpty().MaximumLength(255);
            RuleFor(politician => politician.BirthDate).NotEmpty();
            RuleFor(politician => politician.TwitterUrl).Must(HelperValidatorMethods.IsValidUrl).When(politician => politician.TwitterUrl is not null).WithMessage("Must be a valid url.");
            RuleFor(politician => politician.FacebookUrl).Must(HelperValidatorMethods.IsValidUrl).When(politician => politician.FacebookUrl is not null).WithMessage("Must be a valid url.");
            RuleFor(politician => politician.InstagramUrl).Must(HelperValidatorMethods.IsValidUrl).When(politician => politician.InstagramUrl is not null).WithMessage("Must be a valid url.");
        }
    }
}
