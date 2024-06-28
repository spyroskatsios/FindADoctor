using FluentValidation;

namespace Appointments.Application.Appointments.Commands;

public class BookAppointmentValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentValidator()
    {
        RuleFor(x => x.DateTime)
            .Must(x => x > DateTime.UtcNow);
    }
}