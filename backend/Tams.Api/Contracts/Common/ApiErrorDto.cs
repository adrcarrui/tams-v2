namespace Tams.Api.Contracts.Common;

public sealed class ApiErrorDto
{
    public required string Code { get; init; }

    public required string Message { get; init; }
}