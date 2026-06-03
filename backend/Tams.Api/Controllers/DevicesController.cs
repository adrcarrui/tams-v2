using Microsoft.AspNetCore.Mvc;
using Tams.Api.Application.Devices;
using Tams.Api.Contracts.Common;
using Tams.Api.Contracts.Devices;

namespace Tams.Api.Controllers;

[ApiController]
[Route("api/devices")]
public sealed class DevicesController : ControllerBase
{
    private readonly DeviceService _deviceService;

    public DevicesController(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<DeviceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDevices(
        [FromQuery] GetDevicesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var devices = await _deviceService.GetDevicesAsync(request, cancellationToken);

            return Ok(devices);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                code = "INVALID_DEVICE_FILTER",
                message = exception.Message
            });
        }
    }
}