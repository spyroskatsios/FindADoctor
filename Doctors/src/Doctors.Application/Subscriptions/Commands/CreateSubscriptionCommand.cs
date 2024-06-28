using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace Doctors.Application.Subscriptions.Commands;

public record CreateSubscriptionCommand(SubscriptionType SubscriptionType, Guid DoctorId) : IRequest<ErrorOr<Subscription>>;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly IDoctorWriteRepository _doctorWriteRepository;

    public CreateSubscriptionHandler(IDoctorWriteRepository doctorWriteRepository)
    {
        _doctorWriteRepository = doctorWriteRepository;
    }
    public async Task<ErrorOr<Subscription>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorWriteRepository.GetAsync(DoctorId.From(request.DoctorId), cancellationToken);
        
        if(doctor is null)
            return Error.NotFound(description: "Doctor not found");
        
        var subscription = new Subscription(request.SubscriptionType, DoctorId.From(request.DoctorId));
        
        var result = doctor.AddSubscription(subscription);

        if (result.IsError)
            return result.Errors;

        await _doctorWriteRepository.UpdateAsync(doctor, cancellationToken);
        
        return subscription;

    }
}