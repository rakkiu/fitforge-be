using FitForge.Core.Enums;

namespace FitForge.Core.ValueObjects;

public sealed class UserProfile
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public FitnessLevel? FitnessLevel { get; set; }
    public List<Goal>? Goals { get; set; }
    public List<Equipment>? EquipmentAvailable { get; set; }
    public string? Limitations { get; set; }
    public string? AvatarUrl { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public int Age
    {
        get
        {
            var today = DateTime.UtcNow;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public bool IsProfileComplete =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        DateOfBirth != default &&
        FitnessLevel.HasValue &&
        Goals is { Count: > 0 };
}
