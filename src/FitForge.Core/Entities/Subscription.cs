using FitForge.Core.Enums;

namespace FitForge.Core.Entities;

public sealed class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? PaymentProvider { get; set; }
    public string? ExternalSubscriptionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }

    public static Subscription CreateFree(Guid userId)
    {
        return new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Tier = SubscriptionTier.Free,
            Status = SubscriptionStatus.Active,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public bool IsActive => Status == SubscriptionStatus.Active || Status == SubscriptionStatus.Trial;

    public bool CanAccessPremiumFeatures =>
        IsActive && (Tier == SubscriptionTier.Premium || Tier == SubscriptionTier.Pro);

    public int AiGenerationsPerHour => Tier switch
    {
        SubscriptionTier.Free => 2,
        SubscriptionTier.Premium => 10,
        SubscriptionTier.Pro => int.MaxValue,
        _ => 2
    };
}
