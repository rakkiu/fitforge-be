using FitForge.Core.Entities;
using FitForge.Core.Enums;
using FitForge.Shared.Results;

namespace FitForge.Core.Interfaces;

public interface IWorkoutPlanRepository
{
    Task<Result<WorkoutPlan>> GetByIdAsync(Guid id);
    Task<Result<IReadOnlyList<WorkoutPlan>>> GetByUserIdAsync(Guid userId, WorkoutPlanStatus? status = null);
    Task<Result<WorkoutPlan>> CreateAsync(WorkoutPlan plan);
    Task<Result<WorkoutPlan>> UpdateAsync(WorkoutPlan plan);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<int>> CountActivePlansByUserIdAsync(Guid userId);
}

public interface IWorkoutSessionRepository
{
    Task<Result<WorkoutSession>> GetByIdAsync(Guid id);
    Task<Result<IReadOnlyList<WorkoutSession>>> GetByPlanIdAsync(Guid planId);
    Task<Result<WorkoutSession>> CreateAsync(WorkoutSession session);
    Task<Result<WorkoutSession>> UpdateAsync(WorkoutSession session);
    Task<Result<bool>> DeleteAsync(Guid id);
}

public interface IExerciseSetRepository
{
    Task<Result<ExerciseSet>> GetByIdAsync(Guid id);
    Task<Result<IReadOnlyList<ExerciseSet>>> GetByWorkoutIdAsync(Guid workoutId);
    Task<Result<ExerciseSet>> CreateAsync(ExerciseSet set);
    Task<Result<ExerciseSet>> UpdateAsync(ExerciseSet set);
    Task<Result<bool>> DeleteAsync(Guid id);
}
