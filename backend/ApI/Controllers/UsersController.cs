using ApI.Middleware;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ApI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken)
    {
        var partnerId = User.Claims.First(x => x.Type == CustomClaimNames.PartnerId).Value;
        return await _userManagementService.GetAllUsersByPartnerIdAsync(partnerId, cancellationToken);
    }
}
