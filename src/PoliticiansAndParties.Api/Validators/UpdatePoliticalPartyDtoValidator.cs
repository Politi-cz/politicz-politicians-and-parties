namespace PoliticiansAndParties.Api.Validators;

public class UpdatePoliticalPartyDtoValidator : AbstractValidator<UpdatePoliticalPartyDto>
{
    public UpdatePoliticalPartyDtoValidator()
    {
        _ = RuleFor(p => p.Name).NotEmpty().MaximumLength(255);
        _ = RuleFor(p => p.ImageUrl).NotEmpty().Must(HelperValidatorMethods.IsValidUrl);
        _ = RuleFor(p => p.Tags).NotEmpty();
        _ = RuleForEach(p => p.Tags).NotEmpty();
    }
}
