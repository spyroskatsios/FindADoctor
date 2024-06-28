using FluentValidation;

namespace Doctors.Application.Offices.Commands;

public class CreateOfficeValidator : AbstractValidator<CreateOfficeCommand>
{
    public CreateOfficeValidator()
    {
        RuleFor(x => x.State)
            .MinimumLength(3);
        
        RuleFor(x => x.City)
            .MinimumLength(3);
        
        RuleFor(x => x.Street)
            .MinimumLength(3);
        
        RuleFor(x => x.StreetNumber)
            .MinimumLength(1);
        
        RuleFor(x => x.ZipCode)
            .MinimumLength(3);
    }
}