using FitForge.Core.Entities;
using FitForge.Core.Interfaces;
using FitForge.Shared.Results;

namespace FitForge.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResult>> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var existsResult = await _userRepository.ExistsByEmailAsync(email);
        if (existsResult.IsSuccess && existsResult.Value)
            return Result<AuthResult>.Failure(Error.Conflict("EMAIL_EXISTS", "Email already registered"));

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, 12);
        var user = User.Create(email, passwordHash, firstName, lastName);

        var createResult = await _userRepository.CreateAsync(user);
        if (createResult.IsFailure)
            return Result<AuthResult>.Failure(createResult.Error!);

        var subscription = Subscription.CreateFree(user.Id);
        await _subscriptionRepository.CreateAsync(subscription);

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Result<AuthResult>.Success(new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user
        });
    }

    public async Task<Result<AuthResult>> LoginAsync(string email, string password)
    {
        var userResult = await _userRepository.GetByEmailAsync(email);
        if (userResult.IsFailure)
            return Result<AuthResult>.Failure(Error.Unauthorized("INVALID_CREDENTIALS", "Invalid email or password"));

        var user = userResult.Value!;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Result<AuthResult>.Failure(Error.Unauthorized("INVALID_CREDENTIALS", "Invalid email or password"));

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Result<AuthResult>.Success(new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user
        });
    }

    public async Task<Result<AuthResult>> RefreshTokenAsync(string refreshToken)
    {
        var userId = _jwtService.GetUserIdFromToken(refreshToken);
        if (userId is null)
            return Result<AuthResult>.Failure(Error.Unauthorized("INVALID_TOKEN", "Invalid refresh token"));

        var userResult = await _userRepository.GetByIdAsync(userId.Value);
        if (userResult.IsFailure)
            return Result<AuthResult>.Failure(Error.Unauthorized("USER_NOT_FOUND", "User not found"));

        var user = userResult.Value!;
        var newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        return Result<AuthResult>.Success(new AuthResult
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = user
        });
    }

    public async Task<Result<User>> GetCurrentUserAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }
}
