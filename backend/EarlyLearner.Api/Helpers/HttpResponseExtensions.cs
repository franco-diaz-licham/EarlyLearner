using EarlyLearner.Api.Models;
using EarlyLearner.Shared.Utilities;

namespace EarlyLearner.Api.Helpers;

public static class HttpResponseExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> service response into an appropriate Minimal API <see cref="IResult"/> with the correct HTTP status code.
    /// On success, wraps the value in an <see cref="ApiResponse"/> and returns the corresponding result type (e.g., Ok, Created, Accepted).
    /// On failure, returns an error response with the appropriate HTTP status code and error message.
    /// Any thrown exception will be handled by middleware.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The service result to convert.</param>
    /// <param name="locationUrl">Optional location URL for created or accepted responses.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
    public static IResult ToApiResult<T>(this Result<T> result, string? locationUrl = null)
    {
        var code = ApiResponse.GetHTTPCode(result.Type);

        if (result.IsSuccess) {
            if (result.Value is null) return Results.NoContent();
            var response = new ApiResponse(statusCode: code, data: result.Value);

            return code switch {
                StatusCodes.Status200OK => Results.Ok(response),
                StatusCodes.Status201Created => locationUrl is null
                    ? Results.Json(data: response, statusCode: StatusCodes.Status201Created)
                    : Results.Created(uri: locationUrl, value: response),
                StatusCodes.Status202Accepted => Results.Accepted(uri: locationUrl, value: response),
                _ => Results.Json(data: response, statusCode: code)
            };
        }

        if (result.Error is null) throw new ArgumentException("There is no error to return...");
        var error = new ApiResponse(statusCode: code, message: result.Error.Message);
        return Results.Json(data: error, statusCode: code);
    }

    public static IResult ToApiResult(this Result result, string? locationUrl = null)
    {
        var code = ApiResponse.GetHTTPCode(result.Type);

        if (result.IsSuccess) {
            var response = new ApiResponse(statusCode: code);
            return code switch {
                StatusCodes.Status200OK => Results.Ok(response),
                StatusCodes.Status201Created => locationUrl is null
                    ? Results.Json(data: response, statusCode: StatusCodes.Status201Created)
                    : Results.Created(uri: locationUrl, value: response),
                StatusCodes.Status202Accepted => Results.Accepted(uri: locationUrl, value: response),
                _ => Results.Json(data: response, statusCode: code)
            };
        }

        if (result.Error is null) throw new ArgumentException("There is no error to return...");
        var error = new ApiResponse(statusCode: code, message: result.Error.Message);
        return Results.Json(data: error, statusCode: code);
    }

    /// <summary>
    /// Converts a paginated service <see cref="Result{List{T}}"/> into an appropriate Minimal API <see cref="IResult"/> with HTTP status code and pagination metadata in the response body.
    /// On success, wraps the paginated data in an <see cref="ApiResponse"/>.
    /// On failure, returns an error response with the corresponding HTTP status code.
    /// Any thrown exception will be handled by middleware.
    /// </summary>
    /// <typeparam name="T">The type of the items in the paginated list.</typeparam>
    /// <param name="result">The service result containing the list of items.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The size of each page.</param>
    /// <param name="totalCount">The total number of items available.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
    public static IResult ToPaginatedApiResult<T>(this Result<List<T>> result, int pageNumber, int pageSize, int totalCount)
    {
        var code = ApiResponse.GetHTTPCode(result.Type);

        if (result.IsSuccess) {
            if (result.Value is null) return Results.NoContent();

            // convert to paginated result
            var paginated = new PagedList<T>(items: result.Value, count: totalCount, pageNumber: pageNumber, pageSize: pageSize);
            var output = new ApiResponse(statusCode: code, data: paginated);
            return Results.Json(data: output, statusCode: code);
        }

        if (result.Error is null) throw new ArgumentException("There is no error to return...");
        var error = new ApiResponse(statusCode: code, message: result.Error.Message);
        return Results.Json(data: error, statusCode: code);
    }
}

