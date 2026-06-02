namespace Tams.Api.Contracts.Departments;

public sealed class DepartmentDto
{
    public int Id { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public bool IsActive { get; init; }
}