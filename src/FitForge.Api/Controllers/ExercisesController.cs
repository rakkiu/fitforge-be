using FitForge.Api.DTOs.Exercise;
using FitForge.Core.Entities;
using FitForge.Core.Enums;
using FitForge.Core.Interfaces;
using FitForge.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitForge.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ExercisesController : ControllerBase
{
    private readonly IExerciseRepository _exerciseRepository;

    public ExercisesController(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _exerciseRepository.GetAllAsync();
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapToResponse)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _exerciseRepository.GetByIdAsync(id);
        return result.Match<IActionResult>(
            onSuccess => Ok(MapToResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize]
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        if (!Enum.TryParse<ExerciseCategory>(category, true, out var categoryEnum))
            return BadRequest(new { error = "Invalid category" });

        var result = await _exerciseRepository.GetByCategoryAsync(categoryEnum);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapToResponse)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize]
    [HttpGet("difficulty/{difficulty}")]
    public async Task<IActionResult> GetByDifficulty(string difficulty)
    {
        if (!Enum.TryParse<DifficultyLevel>(difficulty, true, out var difficultyEnum))
            return BadRequest(new { error = "Invalid difficulty" });

        var result = await _exerciseRepository.GetByDifficultyAsync(difficultyEnum);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapToResponse)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Search query is required" });

        var result = await _exerciseRepository.SearchAsync(q);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapToResponse)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize]
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] string? category = null,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? equipment = null)
    {
        ExerciseCategory? categoryEnum = null;
        DifficultyLevel? difficultyEnum = null;

        if (!string.IsNullOrEmpty(category) && Enum.TryParse<ExerciseCategory>(category, true, out var cat))
            categoryEnum = cat;

        if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<DifficultyLevel>(difficulty, true, out var diff))
            difficultyEnum = diff;

        var result = await _exerciseRepository.FilterAsync(categoryEnum, difficultyEnum, equipment);
        return result.Match<IActionResult>(
            onSuccess => Ok(result.Value!.Select(MapToResponse)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExerciseRequest request)
    {
        if (!Enum.TryParse<ExerciseCategory>(request.Category, true, out var categoryEnum))
            return BadRequest(new { error = "Invalid category" });

        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficultyEnum))
            return BadRequest(new { error = "Invalid difficulty" });

        var existsResult = await _exerciseRepository.ExistsByNameAsync(request.Name);
        if (existsResult.IsSuccess && existsResult.Value)
            return Conflict(new { error = "Exercise name already exists" });

        var exercise = Exercise.Create(
            request.Name,
            categoryEnum,
            difficultyEnum,
            request.Equipment,
            request.Instructions);

        var result = await _exerciseRepository.CreateAsync(exercise);
        return result.Match<IActionResult>(
            onSuccess => CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, MapToResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExerciseRequest request)
    {
        var getResult = await _exerciseRepository.GetByIdAsync(id);
        if (getResult.IsFailure)
            return NotFound(new { error = getResult.Error!.Message });

        if (!Enum.TryParse<ExerciseCategory>(request.Category, true, out var categoryEnum))
            return BadRequest(new { error = "Invalid category" });

        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficultyEnum))
            return BadRequest(new { error = "Invalid difficulty" });

        var existsResult = await _exerciseRepository.ExistsByNameAsync(request.Name, id);
        if (existsResult.IsSuccess && existsResult.Value)
            return Conflict(new { error = "Exercise name already exists" });

        var exercise = getResult.Value!;
        exercise.Update(
            request.Name,
            categoryEnum,
            difficultyEnum,
            request.Equipment,
            request.Instructions);

        var result = await _exerciseRepository.UpdateAsync(exercise);
        return result.Match<IActionResult>(
            onSuccess => Ok(MapToResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isReferencedResult = await _exerciseRepository.IsReferencedByExerciseSetAsync(id);
        if (isReferencedResult.IsSuccess && isReferencedResult.Value)
            return Conflict(new { error = "Cannot delete exercise that is referenced by workout sets" });

        var result = await _exerciseRepository.DeleteAsync(id);
        return result.Match<IActionResult>(
            onSuccess => NoContent(),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    private ExerciseResponse MapToResponse(Exercise exercise)
    {
        return new ExerciseResponse
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Category = exercise.Category.ToString(),
            Difficulty = exercise.Difficulty.ToString(),
            Equipment = exercise.Equipment,
            Instructions = exercise.Instructions,
            CreatedAt = exercise.CreatedAt
        };
    }

    private static int MapErrorToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => 400,
        ErrorType.NotFound => 404,
        ErrorType.Conflict => 409,
        ErrorType.Authorization => 401,
        _ => 500
    };
}
