using FitForge.Api.DTOs.Exercise;
using FluentValidation;

namespace FitForge.Api.Validators;

public class CreateExerciseRequestValidator : AbstractValidator<CreateExerciseRequest>
{
    public CreateExerciseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(c => new[] { "chest", "back", "legs", "shoulders", "arms", "core", "cardio" }.Contains(c.ToLower()))
            .WithMessage("Invalid category. Valid values: chest, back, legs, shoulders, arms, core, cardio");

        RuleFor(x => x.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required")
            .Must(d => new[] { "beginner", "intermediate", "advanced" }.Contains(d.ToLower()))
            .WithMessage("Invalid difficulty. Valid values: beginner, intermediate, advanced");

        RuleFor(x => x.Equipment)
            .MaximumLength(100).WithMessage("Equipment must not exceed 100 characters");

        RuleFor(x => x.Instructions)
            .MaximumLength(5000).WithMessage("Instructions must not exceed 5000 characters");
    }
}

public class UpdateExerciseRequestValidator : AbstractValidator<UpdateExerciseRequest>
{
    public UpdateExerciseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(c => new[] { "chest", "back", "legs", "shoulders", "arms", "core", "cardio" }.Contains(c.ToLower()))
            .WithMessage("Invalid category. Valid values: chest, back, legs, shoulders, arms, core, cardio");

        RuleFor(x => x.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required")
            .Must(d => new[] { "beginner", "intermediate", "advanced" }.Contains(d.ToLower()))
            .WithMessage("Invalid difficulty. Valid values: beginner, intermediate, advanced");

        RuleFor(x => x.Equipment)
            .MaximumLength(100).WithMessage("Equipment must not exceed 100 characters");

        RuleFor(x => x.Instructions)
            .MaximumLength(5000).WithMessage("Instructions must not exceed 5000 characters");
    }
}
