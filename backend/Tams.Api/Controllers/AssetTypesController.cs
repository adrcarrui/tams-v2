using Microsoft.AspNetCore.Mvc;
using Tams.Api.Application.AssetTypes;
using Tams.Api.Contracts.AssetTypes;

namespace Tams.Api.Controllers;

[ApiController]
[Route("api/asset-types")]
public sealed class AssetTypesController : ControllerBase
{
    private readonly AssetTypeService _assetTypeService;

    public AssetTypesController(AssetTypeService assetTypeService)
    {
        _assetTypeService = assetTypeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AssetTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssetTypes(CancellationToken cancellationToken)
    {
        var assetTypes = await _assetTypeService.GetActiveAssetTypesAsync(cancellationToken);

        return Ok(assetTypes);
    }
}