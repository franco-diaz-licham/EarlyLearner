using EarlyLearner.Api.Models;
using EarlyLearner.Api.Helpers;
using EarlyLearner.Application.Features.LearningContext;
using EarlyLearner.Domain.IdentityContext.ValueObjects;
using EarlyLearner.Domain.LearningContext.ValueObjects;
using EarlyLearner.Shared.Enums;
using EarlyLearner.Shared.Utilities;
using FluentValidation;

namespace EarlyLearner.Api.Endpoints;

public static class DailyLogEndpoints
{
    public static IEndpointRouteBuilder MapDailyLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dailyLogs = endpoints.MapGroup("/daily-logs").WithTags("Daily logs");
        dailyLogs.MapGet("/", ListDailyLogs).WithName(nameof(ListDailyLogs));
        dailyLogs.MapGet("/{dailyLogId:guid}", GetDailyLog).WithName(nameof(GetDailyLog));
        dailyLogs.MapPost("/", CreateDailyLog).WithName(nameof(CreateDailyLog));
        dailyLogs.MapDelete("/{dailyLogId:guid}", DeleteDailyLog).WithName(nameof(DeleteDailyLog));

        return endpoints;
    }

    public static async Task<IResult> ListDailyLogs(IDailyLogQueryService queryService, CancellationToken cancellationToken = default)
    {
        var result = await queryService.ListAsync(cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> GetDailyLog(
        Guid dailyLogId,
        IDailyLogQueryService queryService,
        CancellationToken cancellationToken = default)
    {
        if (dailyLogId == Guid.Empty) {
            return Result<DailyLogResponse>
                .Fail("Daily log id is required.", ResultTypeEnum.Invalid)
                .ToApiResult();
        }

        var result = await queryService.GetAsync(new DailyLogId(dailyLogId), cancellationToken);
        return result.ToApiResult();
    }

    public static async Task<IResult> CreateDailyLog(
        CreateDailyLogRequest request,
        IValidator<CreateDailyLogRequest> validator,
        IDailyLogCommandService commandService,
        CancellationToken cancellationToken = default)
    {
        var validation = validator.Validate(request).ToResult();
        if (!validation.IsSuccess) return validation.ToApiResult();

        var command = new CreateDailyLogCommand(
            ChildId: new ChildId(request.ChildId),
            LogDate: request.LogDate);

        var result = await commandService.CreateAsync(command, cancellationToken);
        var locationUrl = result.IsSuccess ? $"/daily-logs/{result.Value.DailyLogId}" : null;
        return result.ToApiResult(locationUrl);
    }

    public static async Task<IResult> DeleteDailyLog(
        Guid dailyLogId,
        IDailyLogCommandService commandService,
        CancellationToken cancellationToken = default)
    {
        if (dailyLogId == Guid.Empty) {
            return Result.Fail("Daily log id is required.", ResultTypeEnum.Invalid).ToApiResult();
        }

        var result = await commandService.DeleteAsync(new DailyLogId(dailyLogId), cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record CreateDailyLogRequest(Guid ChildId, DateOnly LogDate);

public sealed class CreateDailyLogRequestValidator : AbstractValidator<CreateDailyLogRequest>
{
    public CreateDailyLogRequestValidator()
    {
        RuleFor(request => request.ChildId).NotEmpty();
        RuleFor(request => request.LogDate).NotEqual(default(DateOnly));
    }
}
