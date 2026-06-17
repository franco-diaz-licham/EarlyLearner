using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.LearningRecordContext;
using Microsoft.AspNetCore.Mvc;

namespace EarlyLearner.Api.Endpoints;

public static class LearningRecordEndpoints
{
    public static IEndpointRouteBuilder MapLearningRecordEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dailyLogs = endpoints.MapGroup("/daily-logs").WithTags("Daily logs");

        dailyLogs.MapGet("/", async (Guid householdId, IDailyLogQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.ListAsync(householdId, cancellationToken)).ToApiResult());

        dailyLogs.MapGet("/{dailyLogId:guid}", async (Guid dailyLogId, IDailyLogQueryService queryService, CancellationToken cancellationToken) =>
            (await queryService.GetAsync(dailyLogId, cancellationToken)).ToApiResult());

        dailyLogs.MapPost("/", async ([FromBody] CreateDailyLogCommand command, IDailyLogCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.CreateAsync(command, cancellationToken)).ToApiResult());

        dailyLogs.MapDelete("/{dailyLogId:guid}", async (Guid dailyLogId, IDailyLogCommandService commandService, CancellationToken cancellationToken) =>
            (await commandService.DeleteAsync(dailyLogId, cancellationToken)).ToApiResult());

        return endpoints;
    }
}
