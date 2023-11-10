using FluentValidation.Results;

namespace Application.Extensions
{
    // Static class for Mediator extensions
    public static class MediatorExtensions
    {
        // Extension method to map validation failures to errors
        public static IEnumerable<IError> MapValidationFailuresToErrors(this IEnumerable<ValidationFailure> failures)
        {
            return failures.Select(f => new Error(f.ErrorMessage));
        }
    }
}
