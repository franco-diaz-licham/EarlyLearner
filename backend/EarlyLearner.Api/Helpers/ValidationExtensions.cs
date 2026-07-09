using EarlyLearner.Shared.Utilities;
using FluentValidation.Results;

namespace EarlyLearner.Api.Helpers;

public static class ValidationExtensions
{
    public static Result ToResult(this ValidationResult validationResult)
    {
        if (validationResult.IsValid) return Result.Success();

        var message = string.Join(
            separator: " ",
            values: validationResult.Errors.Select(error => error.ErrorMessage));

        return Result.Fail(message, ResultTypeEnum.Invalid);
    }
}
