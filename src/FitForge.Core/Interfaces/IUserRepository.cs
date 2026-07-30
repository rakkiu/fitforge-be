using FitForge.Core.Entities;
using FitForge.Shared.Results;

namespace FitForge.Core.Interfaces;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<User>> CreateAsync(User user);
    Task<Result<User>> UpdateAsync(User user);
    Task<Result<bool>> ExistsByEmailAsync(string email);
}
