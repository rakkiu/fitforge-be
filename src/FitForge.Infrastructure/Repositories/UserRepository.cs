using FitForge.Core.Entities;
using FitForge.Core.Interfaces;
using FitForge.Infrastructure.Data;
using FitForge.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace FitForge.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FitForgeDbContext _context;

    public UserRepository(FitForgeDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user is not null
            ? Result<User>.Success(user)
            : Result<User>.Failure(Error.NotFound("USER_NOT_FOUND", "User not found"));
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant().Trim());
        return user is not null
            ? Result<User>.Success(user)
            : Result<User>.Failure(Error.NotFound("USER_NOT_FOUND", "User not found"));
    }

    public async Task<Result<User>> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Result<User>.Success(user);
    }

    public async Task<Result<User>> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result<User>.Success(user);
    }

    public async Task<Result<bool>> ExistsByEmailAsync(string email)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant().Trim());
        return Result<bool>.Success(exists);
    }
}
