namespace FitForge.Api.DTOs.Exercise;

public sealed class ExerciseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string? Equipment { get; set; }
    public string? Instructions { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CreateExerciseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string? Equipment { get; set; }
    public string? Instructions { get; set; }
}

public sealed class UpdateExerciseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string? Equipment { get; set; }
    public string? Instructions { get; set; }
}
