using FluentValidation;

namespace Identity.Core.App.Identity.Commands;

public class RegisterUserValidator : AbstractValidator<RegisterDoctorCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}