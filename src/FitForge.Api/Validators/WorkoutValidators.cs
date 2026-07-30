using FitForge.Api.DTOs.Workout;
using FluentValidation;

namespace FitForge.Api.Validators;

public class CreateWorkoutPlanRequestValidator : AbstractValidator<CreateWorkoutPlanRequest>
{
    public CreateWorkoutPlanRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.PlanType)
            .NotEmpty().WithMessage("Plan type is required")
            .Must(pt => new[] { "strength", "hypertrophy", "cardio", "flexibility" }.Contains(pt.ToLower()))
            .WithMessage("Invalid plan type. Valid values: strength, hypertrophy, cardio, flexibility");

        RuleFor(x => x.DaysPerWeek)
            .InclusiveBetween(1, 7).WithMessage("Days per week must be between 1 and 7");

        RuleFor(x => x.TotalWeeks)
            .InclusiveBetween(1, 52).WithMessage("Total weeks must be between 1 and 52");
    }
}

public class UpdateWorkoutPlanRequestValidator : AbstractValidator<UpdateWorkoutPlanRequest>
{
    public UpdateWorkoutPlanRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.PlanType)
            .NotEmpty().WithMessage("Plan type is required")
            .Must(pt => new[] { "strength", "hypertrophy", "cardio", "flexibility" }.Contains(pt.ToLower()))
            .WithMessage("Invalid plan type. Valid values: strength, hypertrophy, cardio, flexibility");

        RuleFor(x => x.DaysPerWeek)
            .InclusiveBetween(1, 7).WithMessage("Days per week must be between 1 and 7");

        RuleFor(x => x.TotalWeeks)
            .InclusiveBetween(1, 52).WithMessage("Total weeks must be between 1 and 52");
    }
}

public class CreateWorkoutSessionRequestValidator : AbstractValidator<CreateWorkoutSessionRequest>
{
    public CreateWorkoutSessionRequestValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(1, 7).WithMessage("Day of week must be between 1 (Monday) and 7 (Sunday)");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be non-negative");
    }
}

public class CreateExerciseSetRequestValidator : AbstractValidator<CreateExerciseSetRequest>
{
    public CreateExerciseSetRequestValidator()
    {
        RuleFor(x => x.ExerciseId)
            .NotEmpty().WithMessage("Exercise ID is required");

        RuleFor(x => x.SetNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Set number must be at least 1");

        RuleFor(x => x.Reps)
            .Null().When(x => x.Reps is null)
            .InclusiveBetween(0, 99).When(x => x.Reps.HasValue)
            .WithMessage("Reps must be between 0 and 99");

        RuleFor(x => x.WeightKg)
            .Null().When(x => x.WeightKg is null)
            .InclusiveBetween(0, 999).When(x => x.WeightKg.HasValue)
            .WithMessage("Weight must be between 0 and 999 kg");
    }
}
