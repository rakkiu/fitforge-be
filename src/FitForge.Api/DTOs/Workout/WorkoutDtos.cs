namespace FitForge.Api.DTOs.Workout;

public sealed class WorkoutPlanResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public int DaysPerWeek { get; set; }
    public int TotalWeeks { get; set; }
    public string Status { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<WorkoutSessionResponse> Sessions { get; set; } = new();
}

public sealed class CreateWorkoutPlanRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public int DaysPerWeek { get; set; }
    public int TotalWeeks { get; set; }
}

public sealed class UpdateWorkoutPlanRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public int DaysPerWeek { get; set; }
    public int TotalWeeks { get; set; }
}

public sealed class UpdateWorkoutPlanStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public sealed class WorkoutSessionResponse
{
    public Guid Id { get; set; }
    public int DayOfWeek { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int? DurationMinutes { get; set; }
    public int? CaloriesBurned { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ExerciseSetResponse> ExerciseSets { get; set; } = new();
}

public sealed class CreateWorkoutSessionRequest
{
    public int DayOfWeek { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public sealed class ExerciseSetResponse
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int SetNumber { get; set; }
    public int? Reps { get; set; }
    public decimal? WeightKg { get; set; }
    public bool Completed { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreateExerciseSetRequest
{
    public Guid ExerciseId { get; set; }
    public int SetNumber { get; set; }
    public int? Reps { get; set; }
    public decimal? WeightKg { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateExerciseSetRequest
{
    public int? Reps { get; set; }
    public decimal? WeightKg { get; set; }
    public string? Notes { get; set; }
}
