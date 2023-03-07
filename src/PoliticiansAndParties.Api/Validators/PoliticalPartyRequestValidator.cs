using FluentValidation;
using PoliticiansAndParties.Api.Contracts.Requests;

namespace PoliticiansAndParties.Api.Validators;

public class PoliticalPartyRequestValidator : AbstractValidator<PoliticalPartyRequest>
{
    public PoliticalPartyRequestValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(255);
        RuleFor(p => p.ImageUrl).NotEmpty().Must(HelperValidatorMethods.IsValidUrl).WithMessage("Must be a valid url.");
        RuleFor(p => p.Tags).NotEmpty();
        RuleForEach(p => p.Tags).NotEmpty();
        RuleFor(p => p.Politicians).NotEmpty();
        RuleForEach(p => p.Politicians).SetValidator(new PoliticianRequestValidator());
    }
}
