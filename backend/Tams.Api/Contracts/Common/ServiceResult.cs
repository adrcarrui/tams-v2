namespace Tams.Api.Application.Common;

public sealed class ServiceResult<T>
{
    private ServiceResult(T? value, string? errorCode, string? errorMessage)
    {
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public T? Value { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public bool IsSuccess => ErrorCode is null;

    public static ServiceResult<T> Success(T value)
    {
        return new ServiceResult<T>(value, null, null);
    }

    public static ServiceResult<T> Failure(string errorCode, string errorMessage)
    {
        return new ServiceResult<T>(default, errorCode, errorMessage);
    }
}