using FluentValidation;

namespace Doctors.Application.Doctors.Commands;

public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(3)
            .MaximumLength(100);
        
        RuleFor(x => x.LastName)
            .MinimumLength(3)
            .MaximumLength(100);
    }
}