namespace PoliticiansAndParties.Api.Validators;

public class PoliticalPartyRequestValidator : AbstractValidator<PoliticalPartyRequest>
{
    public PoliticalPartyRequestValidator()
    {
        RuleSet("UpdateFields", () =>
        {
            _ = RuleFor(p => p.Name).NotEmpty().MaximumLength(255);
            _ = RuleFor(p => p.ImageUrl).NotEmpty().Must(HelperValidatorMethods.IsValidUrl).WithMessage("Must be a valid url.");
            _ = RuleFor(p => p.Tags).NotEmpty();
            _ = RuleForEach(p => p.Tags).NotEmpty();
        });

        _ = RuleFor(p => p.Politicians).NotEmpty();
        _ = RuleForEach(p => p.Politicians).SetValidator(new PoliticianRequestValidator());
    }
}
