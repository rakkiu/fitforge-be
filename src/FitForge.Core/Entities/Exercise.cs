using System.Text.Json;
using FitForge.Core.Enums;

namespace FitForge.Core.Entities;

public sealed class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExerciseCategory Category { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string? Equipment { get; set; }
    public string? Instructions { get; set; }
    public JsonDocument? MuscleGroup { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static Exercise Create(
        string name,
        ExerciseCategory category,
        DifficultyLevel difficulty,
        string? equipment = null,
        string? instructions = null,
        JsonDocument? muscleGroup = null)
    {
        return new Exercise
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            Difficulty = difficulty,
            Equipment = equipment,
            Instructions = instructions,
            MuscleGroup = muscleGroup,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        ExerciseCategory category,
        DifficultyLevel difficulty,
        string? equipment = null,
        string? instructions = null,
        JsonDocument? muscleGroup = null)
    {
        Name = name;
        Category = category;
        Difficulty = difficulty;
        Equipment = equipment;
        Instructions = instructions;
        MuscleGroup = muscleGroup;
        UpdatedAt = DateTime.UtcNow;
    }
}
