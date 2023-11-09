// Importing the required libraries
using Application.Extensions;

// Namespace for the application pipelines
namespace Application.Pipelines
{
    // Class to handle the validation pipeline for a request
    public class RequestValidationPipeline<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : ResultBase<TResponse>, new()
    {
        // Validator for the request
        private readonly IValidator<TRequest> _requestValidator;
        // Logger for logging information and errors
        private readonly ILogger<RequestValidationPipeline<TRequest, TResponse>> _logger;

        // Constructor for dependency injection
        public RequestValidationPipeline(IValidator<TRequest> requestValidator, ILogger<RequestValidationPipeline<TRequest, TResponse>> logger)
        {
            _requestValidator = requestValidator;
            _logger = logger;
        }

        // Method to handle the request
        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the request
                var validationResult = _requestValidator.Validate(request);

                // If there are any validation errors
                if (validationResult != null && validationResult.Errors.Any())
                {
                    // Map the validation failures to errors
                    var errorList = validationResult.Errors.MapValidationFailuresToErrors();

                    // Create a new response with the errors
                    var errorResponse = new TResponse();
                    return errorResponse.WithErrors(errorList);
                }

                // If there are no validation errors, continue to the next step in the pipeline
                return await next();
            }
            catch (Exception ex)
            {
                // Log the exception message and rethrow the exception
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
