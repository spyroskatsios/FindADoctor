using ErrorOr;

namespace Doctors.Domain.DoctorAggregate;

public static class DoctorErrors
{
    public static readonly Error CannotHaveMoreOfficesThanSubscriptionAllows = Error.Validation(
        "Doctor.CannotHaveMoreOfficesThanSubscriptionAllows",
        "A doctor cannot have more offices than the subscription allows");
    
    public static readonly Error CannotSetSubscriptionIfAlreadyOne = Error.Conflict(
        "Doctor.CannotSetSubscriptionIfAlreadyOne",
        "The doctor already has a Subscription"); 
}