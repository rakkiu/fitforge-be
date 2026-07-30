namespace FitForge.Shared.Results;

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Internal);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Authorization);

    public static Error Internal(string code, string message) =>
        new(code, message, ErrorType.Internal);
}

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Authorization,
    Internal
}
