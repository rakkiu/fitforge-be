using FitForge.Core.Entities;
using FitForge.Core.Enums;
using FitForge.Shared.Results;

namespace FitForge.Core.Interfaces;

public interface IExerciseRepository
{
    Task<Result<Exercise>> GetByIdAsync(Guid id);
    Task<Result<IReadOnlyList<Exercise>>> GetAllAsync();
    Task<Result<IReadOnlyList<Exercise>>> GetByCategoryAsync(ExerciseCategory category);
    Task<Result<IReadOnlyList<Exercise>>> GetByDifficultyAsync(DifficultyLevel difficulty);
    Task<Result<IReadOnlyList<Exercise>>> SearchAsync(string query);
    Task<Result<IReadOnlyList<Exercise>>> FilterAsync(ExerciseCategory? category, DifficultyLevel? difficulty, string? equipment);
    Task<Result<Exercise>> CreateAsync(Exercise exercise);
    Task<Result<Exercise>> UpdateAsync(Exercise exercise);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<bool>> ExistsByNameAsync(string name, Guid? excludeId = null);
    Task<Result<bool>> IsReferencedByExerciseSetAsync(Guid exerciseId);
}
