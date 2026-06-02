using Microsoft.AspNetCore.Mvc;
using Tams.Api.Infrastructure.Data;

namespace Tams.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly TamsDbContext _dbContext;

    public HealthController(TamsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            app = "TAMS API"
        });
    }

    [HttpGet("db")]
    public async Task<IActionResult> GetDatabaseHealth()
    {
        var canConnect = await _dbContext.Database.CanConnectAsync();

        return Ok(new
        {
            status = canConnect ? "ok" : "error",
            database = canConnect ? "reachable" : "unreachable"
        });
    }
}