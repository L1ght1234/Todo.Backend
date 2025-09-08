using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Todo_Backend.BLL.Behaviors.Validation;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
        {
            const string errorMsg = "Validation failed";
            var errorDetails = string.Join("; ", failures.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            _logger.LogError("{ErrorMsg}. Details: {ErrorDetails}", errorMsg, errorDetails);

            var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse).GenericTypeArguments.FirstOrDefault() ?? typeof(Unit));
            var result = Activator.CreateInstance(resultType)!;

            var withErrorsMethod = resultType.GetMethod("WithErrors", new[] { typeof(IEnumerable<string>) });
            withErrorsMethod!.Invoke(result, new object[] { failures.Select(e => e.ErrorMessage) });

            var failMethod = typeof(Result).GetMethods()
                .First(m => m.IsGenericMethod && m.Name == "Fail" && m.GetParameters().Length == 1)
                .MakeGenericMethod(typeof(TResponse).GenericTypeArguments.FirstOrDefault() ?? typeof(Unit));

            return (TResponse)failMethod.Invoke(null, new object[] { errorMsg })!;
        }

        return await next();
    }
}