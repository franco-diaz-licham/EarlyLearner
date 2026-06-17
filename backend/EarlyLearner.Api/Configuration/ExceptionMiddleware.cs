using EarlyLearner.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace EarlyLearner.Api.Configuration;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IHostEnvironment _env;

    /// <summary>
    /// Global middleware for API calls. Catches anything that has not been caught and logs it to server.
    /// </summary>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try {
            await _next(context);
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var devError = new ApiException(statusCode: context.Response.StatusCode, message: ex.Message, details: BuildDevelopmentDetails(ex: ex));
            var prodError = new ApiException(statusCode: context.Response.StatusCode, message: "Error...");
            var response = _env.IsDevelopment() ? devError : prodError;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(value: response, options: options);
            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// This details is only for development.
    /// </summary>
    private static string BuildDevelopmentDetails(Exception ex)
    {
        var messages = FlattenExceptionMessages(ex: ex);
        var dbUpdateDetails = ex is DbUpdateException dbUpdateException ? BuildDbUpdateDetails(ex: dbUpdateException) : null;
        var details = new[] { messages, dbUpdateDetails, ex.StackTrace };
        return string.Join(
            separator: Environment.NewLine + Environment.NewLine,
            values: details.Where(detail => !string.IsNullOrWhiteSpace(detail)));
    }

    private static string FlattenExceptionMessages(Exception ex)
    {
        var messages = new List<string>();
        for (var current = ex; current is not null; current = current.InnerException) messages.Add($"{current.GetType().Name}: {current.Message}");
        return string.Join(separator: Environment.NewLine, values: messages);
    }

    private static string? BuildDbUpdateDetails(DbUpdateException ex)
    {
        var postgresException = ex.GetBaseException() as PostgresException;
        if (postgresException is null) return null;

        var details = new List<string> {
            $"Postgres SQL state: {postgresException.SqlState}",
            $"Severity: {postgresException.Severity}"
        };

        if (!string.IsNullOrWhiteSpace(postgresException.TableName)) details.Add($"Table: {postgresException.TableName}");
        if (!string.IsNullOrWhiteSpace(postgresException.ColumnName)) details.Add($"Column: {postgresException.ColumnName}");
        if (!string.IsNullOrWhiteSpace(postgresException.ConstraintName)) details.Add($"Constraint: {postgresException.ConstraintName}");
        if (!string.IsNullOrWhiteSpace(postgresException.Detail)) details.Add($"Detail: {postgresException.Detail}");

        return string.Join(separator: Environment.NewLine, values: details);
    }
}
