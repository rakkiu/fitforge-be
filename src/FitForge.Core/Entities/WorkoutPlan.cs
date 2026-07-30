using FitForge.Core.Enums;

namespace FitForge.Core.Entities;

public sealed class WorkoutPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkoutType PlanType { get; set; }
    public int DaysPerWeek { get; set; }
    public int TotalWeeks { get; set; }
    public WorkoutPlanStatus Status { get; set; } = WorkoutPlanStatus.Draft;
    public GeneratedBy GeneratedBy { get; set; } = GeneratedBy.Manual;
    public object? AiMetadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User? User { get; set; }

    private readonly List<WorkoutSession> _sessions = new();
    public IReadOnlyCollection<WorkoutSession> Sessions => _sessions.AsReadOnly();

    public static WorkoutPlan Create(
        Guid userId,
        string title,
        WorkoutType planType,
        int daysPerWeek,
        int totalWeeks,
        string? description = null,
        GeneratedBy generatedBy = GeneratedBy.Manual)
    {
        return new WorkoutPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Description = description,
            PlanType = planType,
            DaysPerWeek = daysPerWeek,
            TotalWeeks = totalWeeks,
            Status = WorkoutPlanStatus.Draft,
            GeneratedBy = generatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public bool CanTransitionTo(WorkoutPlanStatus newStatus)
    {
        return Status switch
        {
            WorkoutPlanStatus.Draft => newStatus == WorkoutPlanStatus.Active,
            WorkoutPlanStatus.Active => newStatus == WorkoutPlanStatus.Paused || newStatus == WorkoutPlanStatus.Completed,
            WorkoutPlanStatus.Paused => newStatus == WorkoutPlanStatus.Active,
            WorkoutPlanStatus.Completed => false,
            _ => false
        };
    }

    public void TransitionTo(WorkoutPlanStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum GeneratedBy
{
    Manual = 0,
    Ai = 1
}
