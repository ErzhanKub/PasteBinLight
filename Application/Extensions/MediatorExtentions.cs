using FluentValidation.Results;

namespace Application.Extensions;

public static class MediatorExtentions
{
    public static IEnumerable<IError> MapToErrors(this IEnumerable<ValidationFailure> failures)
        => failures.Select(f => new Error(f.ErrorMessage));
}