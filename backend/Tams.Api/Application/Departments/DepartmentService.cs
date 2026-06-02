using Microsoft.EntityFrameworkCore;
using Tams.Api.Contracts.Departments;
using Tams.Api.Infrastructure.Data;

namespace Tams.Api.Application.Departments;

public sealed class DepartmentService
{
    private readonly TamsDbContext _dbContext;

    public DepartmentService(TamsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetActiveDepartmentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}