namespace FitForge.Core.Entities;

public sealed class ExerciseSet
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public Guid ExerciseId { get; set; }
    public int SetNumber { get; set; }
    public int? Reps { get; set; }
    public decimal? WeightKg { get; set; }
    public bool Completed { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public WorkoutSession? Workout { get; set; }
    public Exercise? Exercise { get; set; }

    public static ExerciseSet Create(
        Guid workoutId,
        Guid exerciseId,
        int setNumber,
        int? reps = null,
        decimal? weightKg = null,
        string? notes = null)
    {
        return new ExerciseSet
        {
            Id = Guid.NewGuid(),
            WorkoutId = workoutId,
            ExerciseId = exerciseId,
            SetNumber = setNumber,
            Reps = reps,
            WeightKg = weightKg,
            Completed = false,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkComplete()
    {
        Completed = true;
    }

    public bool CanEdit()
    {
        return (DateTime.UtcNow - CreatedAt).TotalHours < 24;
    }
}
