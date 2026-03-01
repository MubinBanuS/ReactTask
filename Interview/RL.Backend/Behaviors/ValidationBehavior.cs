namespace RL.Backend.Behaviors;
/// <summary>
/// Defines a pipeline behavior that validates incoming requests using all registered validators before passing the
/// request to the next handler in the pipeline.
/// </summary>
/// <remarks>If any validation failures are detected, a ValidationException is thrown containing details of all
/// validation errors. This ensures that only requests passing validation are processed further in the
/// pipeline.</remarks>
/// <typeparam name="TRequest">The type of the request to be validated. This type must be a non-nullable reference type.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Provides the collection of validators that are used to validate requests of type TRequest.
    /// </summary>
    /// <remarks>This collection contains instances of IValidator<TRequest>, each responsible for enforcing
    /// specific validation rules on the request data. Validators in this collection may implement different validation
    /// strategies, allowing for flexible and extensible request validation.</remarks>
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    /// <summary>
    /// Initializes a new instance of the ValidationBehavior class using the specified collection of validators.
    /// </summary>
    /// <remarks>The validators are executed in the order provided. All validators must pass for the request
    /// to be considered valid.</remarks>
    /// <param name="validators">An enumerable collection of IValidator<TRequest> instances that are used to validate incoming requests. Cannot
    /// be null.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    /// <summary>
    /// Validates the specified request using all registered validators and invokes the next handler in the pipeline if
    /// validation succeeds.
    /// </summary>
    /// <remarks>All registered validators are executed before the request is passed to the next handler. If
    /// any validation errors are found, the operation is aborted and a ValidationException is thrown.</remarks>
    /// <param name="request">The request object containing the data to be validated and processed.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <param name="next">A delegate representing the next handler to invoke in the request processing pipeline.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response returned by the next
    /// handler in the pipeline.</returns>
    /// <exception cref="ValidationException">Thrown when one or more validation failures occur during request validation.</exception>
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationFailures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                                    .SelectMany(r => r.Errors).Where(f => f != null).ToList();
            if (validationFailures.Count != 0)
            {
                throw new ValidationException(validationFailures);
            }
        }
        return await next();
    }
}
