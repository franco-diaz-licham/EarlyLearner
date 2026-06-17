using EarlyLearner.Shared.Enums;

namespace EarlyLearner.Shared.Utilities;

/// <summary>
/// Represents the outcome of an operation, encapsulating success or failure state, result data, error information, and optional metadata such as total count.
/// Used to provide consistent and informative responses from application and service layers.
/// </summary>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public ResultTypeEnum Type { get; set; }
    public T Value { get; }
    public int ValueCount => Value switch {
        null => 0,
        ICollection<T> c => c.Count,
        IEnumerable<object> e => e.Count(),
        _ => 1
    };
    public int TotalCount { get; set; }

    public AppError? Error { get; }

    private Result(bool ok, ResultTypeEnum type, int totalCount = 0, T value = default!, AppError? error = null)
    {
        IsSuccess = ok;
        Type = type;
        TotalCount = totalCount;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value, ResultTypeEnum type, int totalCount = 1) => new(true, type, totalCount, value);
    public static Result<T> Fail(string message, ResultTypeEnum type) => new(false, type, error: new AppError(message));
}

public sealed class Result
{
    public bool IsSuccess { get; }
    public ResultTypeEnum Type { get; }
    public AppError? Error { get; }

    private Result(bool isSuccess, ResultTypeEnum type, AppError? error = null)
    {
        IsSuccess = isSuccess;
        Type = type;
        Error = error;
    }

    public static Result Success(ResultTypeEnum type = ResultTypeEnum.Success) => new(true, type);
    public static Result Fail(string message, ResultTypeEnum type = ResultTypeEnum.Invalid) => new(false, type, new AppError(message));
}

/// <summary>
/// Represents an application error with a descriptive message.
/// Used to encapsulate error information in operation results and responses.
/// </summary>
public sealed record AppError(string Message);