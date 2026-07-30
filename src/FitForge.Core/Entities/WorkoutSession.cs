namespace FitForge.Core.Entities;

public sealed class WorkoutSession
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public int DayOfWeek { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int? DurationMinutes { get; set; }
    public int? CaloriesBurned { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public WorkoutPlan? Plan { get; set; }

    private readonly List<ExerciseSet> _exerciseSets = new();
    public IReadOnlyCollection<ExerciseSet> ExerciseSets => _exerciseSets.AsReadOnly();

    public static WorkoutSession Create(
        Guid planId,
        int dayOfWeek,
        DateTime date,
        string title,
        int orderIndex,
        string? description = null)
    {
        return new WorkoutSession
        {
            Id = Guid.NewGuid(),
            PlanId = planId,
            DayOfWeek = dayOfWeek,
            Date = date,
            Title = title,
            Description = description,
            OrderIndex = orderIndex,
            Completed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkComplete()
    {
        Completed = true;
        CompletedAt = DateTime.UtcNow;
    }

    public bool AllSetsCompleted => ExerciseSets.All(s => s.Completed);
}
