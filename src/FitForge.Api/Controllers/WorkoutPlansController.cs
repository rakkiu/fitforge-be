using System.Security.Claims;
using FitForge.Api.DTOs.Workout;
using FitForge.Core.Entities;
using FitForge.Core.Enums;
using FitForge.Core.Interfaces;
using FitForge.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitForge.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public sealed class WorkoutPlansController : ControllerBase
{
    private readonly IWorkoutPlanRepository _planRepository;
    private readonly IWorkoutSessionRepository _sessionRepository;
    private readonly IExerciseSetRepository _setRepository;

    public WorkoutPlansController(
        IWorkoutPlanRepository planRepository,
        IWorkoutSessionRepository sessionRepository,
        IExerciseSetRepository setRepository)
    {
        _planRepository = planRepository;
        _sessionRepository = sessionRepository;
        _setRepository = setRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        WorkoutPlanStatus? statusEnum = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<WorkoutPlanStatus>(status, true, out var parsed))
            statusEnum = parsed;

        var result = await _planRepository.GetByUserIdAsync(userId.Value, statusEnum);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapToResponse)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _planRepository.GetByIdAsync(id);
        return result.Match<IActionResult>(
            onSuccess => Ok(MapToResponse(result.Value!)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkoutPlanRequest request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (!Enum.TryParse<WorkoutType>(request.PlanType, true, out var planTypeEnum))
            return BadRequest(new { error = "Invalid plan type" });

        var activeCountResult = await _planRepository.CountActivePlansByUserIdAsync(userId.Value);
        if (activeCountResult.IsSuccess && activeCountResult.Value > 0)
            return BadRequest(new { error = "Only one active workout plan allowed at a time" });

        var plan = WorkoutPlan.Create(
            userId.Value,
            request.Title,
            planTypeEnum,
            request.DaysPerWeek,
            request.TotalWeeks,
            request.Description);

        var result = await _planRepository.CreateAsync(plan);
        return result.Match<IActionResult>(
            onSuccess => CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, MapToResponse(result.Value!)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkoutPlanRequest request)
    {
        var getResult = await _planRepository.GetByIdAsync(id);
        if (getResult.IsFailure)
            return NotFound(new { error = getResult.Error!.Message });

        var plan = getResult.Value!;
        if (!Enum.TryParse<WorkoutType>(request.PlanType, true, out var planTypeEnum))
            return BadRequest(new { error = "Invalid plan type" });

        plan.Title = request.Title;
        plan.Description = request.Description;
        plan.PlanType = planTypeEnum;
        plan.DaysPerWeek = request.DaysPerWeek;
        plan.TotalWeeks = request.TotalWeeks;

        var result = await _planRepository.UpdateAsync(plan);
        return result.Match<IActionResult>(
            onSuccess => Ok(MapToResponse(result.Value!)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateWorkoutPlanStatusRequest request)
    {
        var getResult = await _planRepository.GetByIdAsync(id);
        if (getResult.IsFailure)
            return NotFound(new { error = getResult.Error!.Message });

        var plan = getResult.Value!;
        if (!Enum.TryParse<WorkoutPlanStatus>(request.Status, true, out var statusEnum))
            return BadRequest(new { error = "Invalid status" });

        if (!plan.CanTransitionTo(statusEnum))
            return BadRequest(new { error = $"Cannot transition from {plan.Status} to {statusEnum}" });

        plan.TransitionTo(statusEnum);
        var result = await _planRepository.UpdateAsync(plan);
        return result.Match<IActionResult>(
            onSuccess => Ok(MapToResponse(result.Value!)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _planRepository.DeleteAsync(id);
        return result.Match<IActionResult>(
            onSuccess => NoContent(),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpGet("{id:guid}/sessions")]
    public async Task<IActionResult> GetSessions(Guid id)
    {
        var result = await _sessionRepository.GetByPlanIdAsync(id);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapSessionToResponse)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpPost("{id:guid}/sessions")]
    public async Task<IActionResult> CreateSession(Guid id, [FromBody] CreateWorkoutSessionRequest request)
    {
        var planResult = await _planRepository.GetByIdAsync(id);
        if (planResult.IsFailure)
            return NotFound(new { error = planResult.Error!.Message });

        var session = WorkoutSession.Create(
            id,
            request.DayOfWeek,
            request.Date,
            request.Title,
            request.OrderIndex,
            request.Description);

        var result = await _sessionRepository.CreateAsync(session);
        return result.Match<IActionResult>(
            onSuccess => CreatedAtAction(nameof(GetSessions), new { id }, MapSessionToResponse(result.Value!)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpGet("sessions/{sessionId:guid}/sets")]
    public async Task<IActionResult> GetSessionSets(Guid sessionId)
    {
        var result = await _setRepository.GetByWorkoutIdAsync(sessionId);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapSetToResponse)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpPost("sessions/{sessionId:guid}/sets")]
    public async Task<IActionResult> CreateSessionSet(Guid sessionId, [FromBody] CreateExerciseSetRequest request)
    {
        var sessionResult = await _sessionRepository.GetByIdAsync(sessionId);
        if (sessionResult.IsFailure)
            return NotFound(new { error = sessionResult.Error!.Message });

        var set = ExerciseSet.Create(
            sessionId,
            request.ExerciseId,
            request.SetNumber,
            request.Reps,
            request.WeightKg,
            request.Notes);

        var result = await _setRepository.CreateAsync(set);
        return result.Match<IActionResult>(
            onSuccess => Ok(MapSetToResponse(result.Value!)),
            onFailure => Problem(detail: onFailure.Message, statusCode: MapErrorToStatusCode(onFailure.Type)));
    }

    [HttpPatch("sets/{setId:guid}/complete")]
    public async Task<IActionResult> CompleteSet(Guid setId)
    {
        var getResult = await _setRepository.GetByIdAsync(setId);
        if (getResult.IsFailure)
            return NotFound(new { error = getResult.Error!.Message });

        var set = getResult.Value!;
        set.MarkComplete();

        var updateResult = await _setRepository.UpdateAsync(set);
        if (updateResult.IsFailure)
            return Problem(detail: updateResult.Error!.Message, statusCode: 500);

        var sessionResult = await _sessionRepository.GetByIdAsync(set.WorkoutId);
        if (sessionResult.IsSuccess && sessionResult.Value!.AllSetsCompleted)
        {
            sessionResult.Value.MarkComplete();
            await _sessionRepository.UpdateAsync(sessionResult.Value);
        }

        return Ok(MapSetToResponse(set));
    }

    private WorkoutPlanResponse MapToResponse(WorkoutPlan plan)
    {
        return new WorkoutPlanResponse
        {
            Id = plan.Id,
            Title = plan.Title,
            Description = plan.Description,
            PlanType = plan.PlanType.ToString(),
            DaysPerWeek = plan.DaysPerWeek,
            TotalWeeks = plan.TotalWeeks,
            Status = plan.Status.ToString(),
            GeneratedBy = plan.GeneratedBy.ToString(),
            CreatedAt = plan.CreatedAt,
            Sessions = plan.Sessions.Select(MapSessionToResponse).ToList()
        };
    }

    private WorkoutSessionResponse MapSessionToResponse(WorkoutSession session)
    {
        return new WorkoutSessionResponse
        {
            Id = session.Id,
            DayOfWeek = session.DayOfWeek,
            Date = session.Date,
            Title = session.Title,
            Description = session.Description,
            OrderIndex = session.OrderIndex,
            DurationMinutes = session.DurationMinutes,
            CaloriesBurned = session.CaloriesBurned,
            Completed = session.Completed,
            CompletedAt = session.CompletedAt,
            ExerciseSets = session.ExerciseSets.Select(MapSetToResponse).ToList()
        };
    }

    private ExerciseSetResponse MapSetToResponse(ExerciseSet set)
    {
        return new ExerciseSetResponse
        {
            Id = set.Id,
            ExerciseId = set.ExerciseId,
            ExerciseName = set.Exercise?.Name ?? string.Empty,
            SetNumber = set.SetNumber,
            Reps = set.Reps,
            WeightKg = set.WeightKg,
            Completed = set.Completed,
            Notes = set.Notes
        };
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim is not null && Guid.TryParse(claim.Value, out var userId) ? userId : null;
    }

    private static int MapErrorToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => 400,
        ErrorType.NotFound => 404,
        ErrorType.Conflict => 409,
        ErrorType.Authorization => 401,
        _ => 500
    };
}
