namespace FitForge.Shared.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> map)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(map(result.Value!))
            : Result<TOut>.Failure(result.Error!);
    }

    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> map)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(await map(result.Value!))
            : Result<TOut>.Failure(result.Error!);
    }

    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> bind)
    {
        return result.IsSuccess
            ? bind(result.Value!)
            : Result<TOut>.Failure(result.Error!);
    }

    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> bind)
    {
        return result.IsSuccess
            ? await bind(result.Value!)
            : Result<TOut>.Failure(result.Error!);
    }

    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Error error)
    {
        return result.IsSuccess && !predicate(result.Value!)
            ? Result<T>.Failure(error)
            : result;
    }
}
