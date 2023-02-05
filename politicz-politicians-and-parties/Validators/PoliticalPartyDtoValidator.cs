using FluentValidation;
using politicz_politicians_and_parties.Dtos;

namespace politicz_politicians_and_parties.Validators
{
    public class PoliticalPartyDtoValidator : AbstractValidator<PoliticalPartyDto>
    {
        public PoliticalPartyDtoValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(255);
            RuleFor(p => p.ImageUrl).NotEmpty().Must(HelperValidatorMethods.IsValidUrl);
            RuleFor(p => p.Tags).NotEmpty();
            RuleForEach(p => p.Tags).NotEmpty();
            RuleFor(p => p.Politicians).NotEmpty();
            RuleForEach(p => p.Politicians).SetValidator(new PoliticianDtoValidator());
        }
    }
}
