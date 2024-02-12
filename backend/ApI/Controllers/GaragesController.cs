using System.Net;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ApI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GaragesController : ControllerBase
{
    private readonly IGarageManagementService _garageManagementService;

    public GaragesController(IGarageManagementService garageManagementService)
    {
        _garageManagementService = garageManagementService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Garage>), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> GetGarages(CancellationToken cancellationToken)
    {
        return new OkObjectResult(await _garageManagementService.GetAllGaragesAsync(cancellationToken));
    }

    [HttpGet("{garageId}")]
    [ProducesResponseType(typeof(Garage), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetGarage(Guid garageId, CancellationToken cancellationToken)
    {
        var garage = await _garageManagementService.GetGarageByIdAsync(garageId, cancellationToken);

        if (garage == null) return new NotFoundResult();

        return new OkObjectResult(garage);
    }
}
