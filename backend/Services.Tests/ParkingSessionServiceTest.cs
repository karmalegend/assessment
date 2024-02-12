using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Services.Tests;

public class ParkingSessionServiceTest
{
    private readonly IGarageManagementService _garageManagementService;
    private readonly IParkingSessionRepository _parkingSessionRepository;
    private readonly ParkingSessionService _parkingSessionService;
    private readonly IUserManagementService _userManagementService;

    public ParkingSessionServiceTest()
    {
        _garageManagementService = Substitute.For<IGarageManagementService>();
        _parkingSessionRepository = Substitute.For<IParkingSessionRepository>();
        _userManagementService = Substitute.For<IUserManagementService>();

        _parkingSessionService = new ParkingSessionService(_garageManagementService,
            _userManagementService, _parkingSessionRepository);
    }

    [Fact]
    public async Task StartParkingSession_ShouldReturnSessionId_WhenSuccessful()
    {
        var garageId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _garageManagementService.CanAcceptParkingSessions(garageId, cancellationToken)
            .Returns(Task.FromResult(true));
        _userManagementService.UserCanStartNewParkingSessionAsync(userId, cancellationToken)
            .Returns(Task.FromResult(true));

        var parkingSession = new ParkingSession
        {
            Id = Guid.NewGuid(),
            GarageId = garageId,
            StartTime = DateTime.Now,
            UserId = userId,
            SessionsState = ParkingSessionsState.InProgress
        };

        _parkingSessionRepository.CreateParkingSessions(Arg.Any<ParkingSession>(), cancellationToken)
            .Returns(Task.FromResult(parkingSession));

        var result = await _parkingSessionService.StartParkingSession(garageId, userId, cancellationToken);

        await _garageManagementService.Received(1)
            .ReserveParkingSpot(Arg.Is<Guid>(g => g == garageId), Arg.Any<CancellationToken>());

        await _parkingSessionRepository.Received(1)
            .CreateParkingSessions(Arg.Is<ParkingSession>(p => p.UserId == userId), Arg.Any<CancellationToken>());

        await _garageManagementService.Received(1)
            .OpenEntryDoor(Arg.Is<Guid>(g => g == garageId), Arg.Any<CancellationToken>());

        result.Should().Be(parkingSession.Id);
    }

    [Fact]
    public async Task StartParkingSession_ShouldThrowGarageInteractionFailedException_WhenGarageCantAcceptSession()
    {
        var garageId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _garageManagementService.CanAcceptParkingSessions(garageId, cancellationToken)
            .Returns(Task.FromResult(false));

        Func<Task> act = async () =>
            await _parkingSessionService.StartParkingSession(garageId, userId, cancellationToken);

        await act.Should().ThrowAsync<GarageInteractionFailedException>();
    }

    [Fact]
    public async Task
        StartParkingSession_ShouldThrowUserHasRunningParkingSessionException_WhenUserHasAnActiveParkingSession()
    {
        var garageId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _garageManagementService.CanAcceptParkingSessions(garageId, cancellationToken)
            .Returns(Task.FromResult(true));
        _userManagementService.UserCanStartNewParkingSessionAsync(userId, cancellationToken)
            .Returns(Task.FromResult(false));

        Func<Task> act = async () =>
            await _parkingSessionService.StartParkingSession(garageId, userId, cancellationToken);

        await act.Should().ThrowAsync<UserHasRunningParkingSessionException>();
    }


    [Fact]
    public async Task StopParkingSession_ShouldReturnSessionId_WhenSuccess()
    {
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        var parkingSession = new ParkingSession
        {
            Id = sessionId,
            GarageId = Guid.NewGuid(),
            StartTime = DateTime.Now,
            UserId = userId,
            SessionsState = ParkingSessionsState.InProgress
        };

        _parkingSessionRepository.StopParkingSessionById(sessionId, userId, cancellationToken)
            .Returns(Task.FromResult(parkingSession));

        var result = await _parkingSessionService.StopParkingSession(sessionId, userId, cancellationToken);

        result.Should().Be(parkingSession.Id);
    }
}
