using System.Net;
using ApI.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ApI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ParkingSessionController : ControllerBase
{
    private readonly IParkingSessionService _parkingSessionService;

    public ParkingSessionController(IParkingSessionService parkingSessionService)
    {
        _parkingSessionService = parkingSessionService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> CreateParkingSession([FromBody] Guid garageId, CancellationToken cancellationToken)
    {
        var userId = User.Claims.First(x => x.Type == CustomClaimNames.UserId).Value;
        var res = await _parkingSessionService.StartParkingSession(garageId, Guid.Parse(userId), cancellationToken);
        return new OkObjectResult(res);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(Guid), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> StopParkingSession([FromBody] Guid parkingSessionId,
        CancellationToken cancellationToken)
    {
        var userId = User.Claims.First(x => x.Type == CustomClaimNames.UserId).Value;
        var res = await _parkingSessionService.StopParkingSession(parkingSessionId, Guid.Parse(userId),
            cancellationToken);
        return new OkObjectResult(res);
    }
}
