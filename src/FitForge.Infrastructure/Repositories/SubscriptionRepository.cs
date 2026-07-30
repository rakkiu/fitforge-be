using FitForge.Core.Entities;
using FitForge.Core.Interfaces;
using FitForge.Infrastructure.Data;
using FitForge.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace FitForge.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly FitForgeDbContext _context;

    public SubscriptionRepository(FitForgeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Subscription>> GetByIdAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        return subscription is not null
            ? Result<Subscription>.Success(subscription)
            : Result<Subscription>.Failure(Error.NotFound("SUBSCRIPTION_NOT_FOUND", "Subscription not found"));
    }

    public async Task<Result<Subscription>> GetByUserIdAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId);
        return subscription is not null
            ? Result<Subscription>.Success(subscription)
            : Result<Subscription>.Failure(Error.NotFound("SUBSCRIPTION_NOT_FOUND", "Subscription not found"));
    }

    public async Task<Result<Subscription>> CreateAsync(Subscription subscription)
    {
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return Result<Subscription>.Success(subscription);
    }

    public async Task<Result<Subscription>> UpdateAsync(Subscription subscription)
    {
        subscription.UpdatedAt = DateTime.UtcNow;
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
        return Result<Subscription>.Success(subscription);
    }
}
