using Microsoft.AspNetCore.Mvc;
using Tams.Api.Application.Devices;
using Tams.Api.Contracts.Common;
using Tams.Api.Contracts.Devices;
using Tams.Api.Application.Common;

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
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
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
            return BadRequest(new ApiErrorDto
            {
                Code = "INVALID_DEVICE_FILTER",
                Message = exception.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeviceById(
        int id,
        CancellationToken cancellationToken)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id, cancellationToken);

        if (device is null)
        {
            return NotFound(new ApiErrorDto
            {
                Code = "DEVICE_NOT_FOUND",
                Message = "Device was not found."
            });
        }

        return Ok(device);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDevice(
        [FromBody] CreateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _deviceService.CreateDeviceAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiErrorDto
            {
                Code = result.ErrorCode!,
                Message = result.ErrorMessage!
            });
        }

        return CreatedAtAction(
            nameof(GetDeviceById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDevice(
        int id,
        [FromBody] UpdateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _deviceService.UpdateDeviceAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = new ApiErrorDto
            {
                Code = result.ErrorCode!,
                Message = result.ErrorMessage!
            };

            if (result.ErrorCode == "DEVICE_NOT_FOUND")
            {
                return NotFound(error);
            }

            return BadRequest(error);
        }

        return Ok(result.Value);
    }
    [HttpPatch("{id:int}/mark-lost")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkDeviceAsLost(
    int id,
    CancellationToken cancellationToken)
    {
        var result = await _deviceService.MarkDeviceAsLostAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ToErrorResult(result);
        }

        return Ok(result.Value);
    }
    [HttpPatch("{id:int}/annul")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AnnulDevice(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _deviceService.AnnulDeviceAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ToErrorResult(result);
        }

        return Ok(result.Value);
    }
    [HttpPatch("{id:int}/restore")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreDevice(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _deviceService.RestoreDeviceAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ToErrorResult(result);
        }

        return Ok(result.Value);
    }
    private IActionResult ToErrorResult<T>(ServiceResult<T> result)
{
    var error = new ApiErrorDto
    {
        Code = result.ErrorCode!,
        Message = result.ErrorMessage!
    };

    if (result.ErrorCode == "DEVICE_NOT_FOUND")
    {
        return NotFound(error);
    }

    return BadRequest(error);
}
}