using Microsoft.AspNetCore.Mvc;

namespace Tams.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            app = "TAMS API"
        });
    }
}