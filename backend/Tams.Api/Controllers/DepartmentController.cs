using Microsoft.AspNetCore.Mvc;
using Tams.Api.Application.Departments;
using Tams.Api.Contracts.Departments;

namespace Tams.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly DepartmentService _departmentService;

    public DepartmentsController(DepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetActiveDepartmentsAsync(cancellationToken);

        return Ok(departments);
    }
}