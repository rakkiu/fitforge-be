using FitForge.Core.Entities;
using FitForge.Shared.Results;

namespace FitForge.Core.Interfaces;

public interface ISubscriptionRepository
{
    Task<Result<Subscription>> GetByIdAsync(Guid id);
    Task<Result<Subscription>> GetByUserIdAsync(Guid userId);
    Task<Result<Subscription>> CreateAsync(Subscription subscription);
    Task<Result<Subscription>> UpdateAsync(Subscription subscription);
}
