using FitForge.Core.Entities;
using FitForge.Core.Enums;
using FitForge.Core.Interfaces;
using FitForge.Infrastructure.Data;
using FitForge.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace FitForge.Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly FitForgeDbContext _context;

    public ExerciseRepository(FitForgeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Exercise>> GetByIdAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        return exercise is not null
            ? Result<Exercise>.Success(exercise)
            : Result<Exercise>.Failure(Error.NotFound("EXERCISE_NOT_FOUND", "Exercise not found"));
    }

    public async Task<Result<IReadOnlyList<Exercise>>> GetAllAsync()
    {
        var exercises = await _context.Exercises
            .OrderBy(e => e.Name)
            .ToListAsync();
        return Result<IReadOnlyList<Exercise>>.Success(exercises);
    }

    public async Task<Result<IReadOnlyList<Exercise>>> GetByCategoryAsync(ExerciseCategory category)
    {
        var exercises = await _context.Exercises
            .Where(e => e.Category == category)
            .OrderBy(e => e.Name)
            .ToListAsync();
        return Result<IReadOnlyList<Exercise>>.Success(exercises);
    }

    public async Task<Result<IReadOnlyList<Exercise>>> GetByDifficultyAsync(DifficultyLevel difficulty)
    {
        var exercises = await _context.Exercises
            .Where(e => e.Difficulty == difficulty)
            .OrderBy(e => e.Name)
            .ToListAsync();
        return Result<IReadOnlyList<Exercise>>.Success(exercises);
    }

    public async Task<Result<IReadOnlyList<Exercise>>> SearchAsync(string query)
    {
        var exercises = await _context.Exercises
            .Where(e => e.Name.Contains(query) ||
                       (e.Instructions != null && e.Instructions.Contains(query)))
            .OrderBy(e => e.Name)
            .ToListAsync();
        return Result<IReadOnlyList<Exercise>>.Success(exercises);
    }

    public async Task<Result<IReadOnlyList<Exercise>>> FilterAsync(
        ExerciseCategory? category,
        DifficultyLevel? difficulty,
        string? equipment)
    {
        var query = _context.Exercises.AsQueryable();

        if (category.HasValue)
            query = query.Where(e => e.Category == category.Value);

        if (difficulty.HasValue)
            query = query.Where(e => e.Difficulty == difficulty.Value);

        if (!string.IsNullOrEmpty(equipment))
            query = query.Where(e => e.Equipment != null && e.Equipment.Contains(equipment));

        var exercises = await query
            .OrderBy(e => e.Name)
            .ToListAsync();

        return Result<IReadOnlyList<Exercise>>.Success(exercises);
    }

    public async Task<Result<Exercise>> CreateAsync(Exercise exercise)
    {
        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();
        return Result<Exercise>.Success(exercise);
    }

    public async Task<Result<Exercise>> UpdateAsync(Exercise exercise)
    {
        exercise.UpdatedAt = DateTime.UtcNow;
        _context.Exercises.Update(exercise);
        await _context.SaveChangesAsync();
        return Result<Exercise>.Success(exercise);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise is null)
            return Result<bool>.Failure(Error.NotFound("EXERCISE_NOT_FOUND", "Exercise not found"));

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Exercises.Where(e => e.Name == name);
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        var exists = await query.AnyAsync();
        return Result<bool>.Success(exists);
    }

    public async Task<Result<bool>> IsReferencedByExerciseSetAsync(Guid exerciseId)
    {
        var isReferenced = await _context.Database
            .SqlQueryRaw<bool>(
                "SELECT EXISTS(SELECT 1 FROM exercise_sets WHERE exercise_id = {0})",
                exerciseId)
            .FirstOrDefaultAsync();
        return Result<bool>.Success(isReferenced);
    }
}
