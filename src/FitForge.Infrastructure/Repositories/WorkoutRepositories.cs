using FitForge.Core.Entities;
using FitForge.Core.Enums;
using FitForge.Core.Interfaces;
using FitForge.Infrastructure.Data;
using FitForge.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace FitForge.Infrastructure.Repositories;

public class WorkoutPlanRepository : IWorkoutPlanRepository
{
    private readonly FitForgeDbContext _context;

    public WorkoutPlanRepository(FitForgeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WorkoutPlan>> GetByIdAsync(Guid id)
    {
        var plan = await _context.WorkoutPlans
            .Include(wp => wp.Sessions)
            .FirstOrDefaultAsync(wp => wp.Id == id && !wp.IsDeleted);
        return plan is not null
            ? Result<WorkoutPlan>.Success(plan)
            : Result<WorkoutPlan>.Failure(Error.NotFound("WORKOUT_PLAN_NOT_FOUND", "Workout plan not found"));
    }

    public async Task<Result<IReadOnlyList<WorkoutPlan>>> GetByUserIdAsync(Guid userId, WorkoutPlanStatus? status = null)
    {
        var query = _context.WorkoutPlans
            .Where(wp => wp.UserId == userId && !wp.IsDeleted);

        if (status.HasValue)
            query = query.Where(wp => wp.Status == status.Value);

        var plans = await query
            .OrderByDescending(wp => wp.CreatedAt)
            .ToListAsync();

        return Result<IReadOnlyList<WorkoutPlan>>.Success(plans);
    }

    public async Task<Result<WorkoutPlan>> CreateAsync(WorkoutPlan plan)
    {
        _context.WorkoutPlans.Add(plan);
        await _context.SaveChangesAsync();
        return Result<WorkoutPlan>.Success(plan);
    }

    public async Task<Result<WorkoutPlan>> UpdateAsync(WorkoutPlan plan)
    {
        plan.UpdatedAt = DateTime.UtcNow;
        _context.WorkoutPlans.Update(plan);
        await _context.SaveChangesAsync();
        return Result<WorkoutPlan>.Success(plan);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var plan = await _context.WorkoutPlans.FindAsync(id);
        if (plan is null)
            return Result<bool>.Failure(Error.NotFound("WORKOUT_PLAN_NOT_FOUND", "Workout plan not found"));

        plan.SoftDelete();
        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> CountActivePlansByUserIdAsync(Guid userId)
    {
        var count = await _context.WorkoutPlans
            .CountAsync(wp => wp.UserId == userId && wp.Status == WorkoutPlanStatus.Active && !wp.IsDeleted);
        return Result<int>.Success(count);
    }
}

public class WorkoutSessionRepository : IWorkoutSessionRepository
{
    private readonly FitForgeDbContext _context;

    public WorkoutSessionRepository(FitForgeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WorkoutSession>> GetByIdAsync(Guid id)
    {
        var session = await _context.WorkoutSessions
            .Include(ws => ws.ExerciseSets)
            .FirstOrDefaultAsync(ws => ws.Id == id);
        return session is not null
            ? Result<WorkoutSession>.Success(session)
            : Result<WorkoutSession>.Failure(Error.NotFound("WORKOUT_SESSION_NOT_FOUND", "Workout session not found"));
    }

    public async Task<Result<IReadOnlyList<WorkoutSession>>> GetByPlanIdAsync(Guid planId)
    {
        var sessions = await _context.WorkoutSessions
            .Where(ws => ws.PlanId == planId)
            .OrderBy(ws => ws.OrderIndex)
            .ToListAsync();
        return Result<IReadOnlyList<WorkoutSession>>.Success(sessions);
    }

    public async Task<Result<WorkoutSession>> CreateAsync(WorkoutSession session)
    {
        _context.WorkoutSessions.Add(session);
        await _context.SaveChangesAsync();
        return Result<WorkoutSession>.Success(session);
    }

    public async Task<Result<WorkoutSession>> UpdateAsync(WorkoutSession session)
    {
        _context.WorkoutSessions.Update(session);
        await _context.SaveChangesAsync();
        return Result<WorkoutSession>.Success(session);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session is null)
            return Result<bool>.Failure(Error.NotFound("WORKOUT_SESSION_NOT_FOUND", "Workout session not found"));

        _context.WorkoutSessions.Remove(session);
        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}

public class ExerciseSetRepository : IExerciseSetRepository
{
    private readonly FitForgeDbContext _context;

    public ExerciseSetRepository(FitForgeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ExerciseSet>> GetByIdAsync(Guid id)
    {
        var set = await _context.ExerciseSets.FindAsync(id);
        return set is not null
            ? Result<ExerciseSet>.Success(set)
            : Result<ExerciseSet>.Failure(Error.NotFound("EXERCISE_SET_NOT_FOUND", "Exercise set not found"));
    }

    public async Task<Result<IReadOnlyList<ExerciseSet>>> GetByWorkoutIdAsync(Guid workoutId)
    {
        var sets = await _context.ExerciseSets
            .Where(es => es.WorkoutId == workoutId)
            .OrderBy(es => es.SetNumber)
            .ToListAsync();
        return Result<IReadOnlyList<ExerciseSet>>.Success(sets);
    }

    public async Task<Result<ExerciseSet>> CreateAsync(ExerciseSet set)
    {
        _context.ExerciseSets.Add(set);
        await _context.SaveChangesAsync();
        return Result<ExerciseSet>.Success(set);
    }

    public async Task<Result<ExerciseSet>> UpdateAsync(ExerciseSet set)
    {
        _context.ExerciseSets.Update(set);
        await _context.SaveChangesAsync();
        return Result<ExerciseSet>.Success(set);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var set = await _context.ExerciseSets.FindAsync(id);
        if (set is null)
            return Result<bool>.Failure(Error.NotFound("EXERCISE_SET_NOT_FOUND", "Exercise set not found"));

        _context.ExerciseSets.Remove(set);
        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}
