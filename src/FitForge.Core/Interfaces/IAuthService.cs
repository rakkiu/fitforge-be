using FitForge.Core.Entities;
using FitForge.Shared.Results;

namespace FitForge.Core.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResult>> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<Result<AuthResult>> LoginAsync(string email, string password);
    Task<Result<AuthResult>> RefreshTokenAsync(string refreshToken);
    Task<Result<User>> GetCurrentUserAsync(Guid userId);
}

public class AuthResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}
