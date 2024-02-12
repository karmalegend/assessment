using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ApI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Services;
using Xunit;

namespace Api.Tests.Controllers;

public class ParkingSessionControllerTests
{
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly Guid TestGarageId = Guid.NewGuid();
    private static readonly Guid TestSessionId = Guid.NewGuid();
    private readonly ParkingSessionController _parkingSessionController;
    private readonly IParkingSessionService _parkingSessionService;

    public ParkingSessionControllerTests()
    {
        _parkingSessionService = Substitute.For<IParkingSessionService>();
        _parkingSessionController = new ParkingSessionController(_parkingSessionService);

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("UserId", TestUserId.ToString())
                }))
            }
        };
        _parkingSessionController.ControllerContext = controllerContext;
    }

    [Fact]
    public async Task CreateParkingSession_ShouldReturnSessionId_WhenCalled()
    {
        _parkingSessionService.StartParkingSession(TestGarageId, TestUserId, Arg.Any<CancellationToken>())
            .Returns(TestSessionId);

        var result =
            await _parkingSessionController.CreateParkingSession(TestGarageId, CancellationToken.None) as
                OkObjectResult;

        result!.Value.Should().Be(TestSessionId);
    }


    [Fact]
    public async Task StopParkingSession_ShouldReturnSessionId_WhenCalled()
    {
        _parkingSessionService.StopParkingSession(TestSessionId, TestUserId, Arg.Any<CancellationToken>())
            .Returns(TestSessionId);

        var result =
            await _parkingSessionController.StopParkingSession(TestSessionId, CancellationToken.None) as
                OkObjectResult;

        result!.Value.Should().Be(TestSessionId);
    }
}
