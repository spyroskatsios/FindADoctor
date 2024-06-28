using ErrorOr;
using FluentValidation;
using MediatR;

namespace Identity.Core.App.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();
        
        var validationResults = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(request, cancellationToken)));
        
        var failures = validationResults
            .Where(x => x.Errors.Count != 0)
            .SelectMany(x => x.Errors)
            .ToList();
        
        if (failures.Count == 0)
            return await next();
        
        var errors = failures
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

        return (dynamic)errors;
    }
    
}