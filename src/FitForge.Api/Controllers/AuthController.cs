using System.Security.Claims;
using FitForge.Api.DTOs.Auth;
using FitForge.Core.Interfaces;
using FitForge.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitForge.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        return result.Match<IActionResult>(
            onSuccess => Ok(MapToAuthResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);

        return result.Match<IActionResult>(
            onSuccess => Ok(MapToAuthResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        return result.Match<IActionResult>(
            onSuccess => Ok(MapToAuthResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _authService.GetCurrentUserAsync(userId.Value);

        return result.Match<IActionResult>(
            onSuccess => Ok(MapToUserResponse(result.Value!)),
            onFailure => Problem(
                detail: onFailure.Message,
                statusCode: MapErrorToStatusCode(onFailure.Type),
                title: onFailure.Code));
    }

    private AuthResponse MapToAuthResponse(Core.Interfaces.AuthResult authResult)
    {
        return new AuthResponse
        {
            AccessToken = authResult.AccessToken,
            RefreshToken = authResult.RefreshToken,
            User = MapToUserResponse(authResult.User)
        };
    }

    private UserResponse MapToUserResponse(Core.Entities.User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.Profile.FirstName,
            LastName = user.Profile.LastName,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId)
            ? userId
            : null;
    }

    private static int MapErrorToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => 400,
        ErrorType.NotFound => 404,
        ErrorType.Conflict => 409,
        ErrorType.Authorization => 401,
        _ => 500
    };
}
