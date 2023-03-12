namespace PoliticiansAndParties.Api.Validators;

public class PoliticianRequestValidator : AbstractValidator<PoliticianRequest>
{
    public PoliticianRequestValidator()
    {
        _ = RuleFor(politician => politician.FullName).NotEmpty().MaximumLength(255);
        _ = RuleFor(politician => politician.BirthDate).NotEmpty();
        _ = RuleFor(politician => politician.ImageUrl).Must(HelperValidatorMethods.IsValidUrl)
            .WithMessage("Must be a valid url.");
        _ = RuleFor(politician => politician.TwitterUrl).Must(HelperValidatorMethods.IsValidUrl)
            .When(politician => politician.TwitterUrl is not null).WithMessage("Must be a valid url.");
        _ = RuleFor(politician => politician.FacebookUrl).Must(HelperValidatorMethods.IsValidUrl)
            .When(politician => politician.FacebookUrl is not null).WithMessage("Must be a valid url.");
        _ = RuleFor(politician => politician.InstagramUrl).Must(HelperValidatorMethods.IsValidUrl)
            .When(politician => politician.InstagramUrl is not null).WithMessage("Must be a valid url.");
    }
}
