using Data;
using Domain;
using Domain.Exceptions;

namespace Services;

public class ParkingSessionService : IParkingSessionService
{
    private readonly IGarageManagementService _garageManagementService;
    private readonly IParkingSessionRepository _parkingSessionRepository;
    private readonly IUserManagementService _userManagementService;

    public ParkingSessionService(IGarageManagementService garageManagementService,
        IUserManagementService userManagementService, IParkingSessionRepository parkingSessionRepository)
    {
        _garageManagementService = garageManagementService;
        _userManagementService = userManagementService;
        _parkingSessionRepository = parkingSessionRepository;
    }

    public async Task<Guid> StartParkingSession(Guid garageId, Guid userId, CancellationToken cancellationToken)
    {
        if (!await _garageManagementService.CanAcceptParkingSessions(garageId, cancellationToken))
            throw new GarageInteractionFailedException();

        if (!await _userManagementService.UserCanStartNewParkingSessionAsync(userId, cancellationToken))
            throw new UserHasRunningParkingSessionException();

        await _garageManagementService.ReserveParkingSpot(garageId, cancellationToken);

        var parkingSession = new ParkingSession
        {
            Id = Guid.NewGuid(), GarageId = garageId, StartTime = DateTime.Now, UserId = userId,
            SessionsState = ParkingSessionsState.InProgress
        };

        var session = await _parkingSessionRepository.CreateParkingSessions(parkingSession, cancellationToken);

        await _garageManagementService.OpenEntryDoor(garageId, cancellationToken);

        return session.Id;
    }

    public async Task<Guid> StopParkingSession(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        var session = await _parkingSessionRepository.StopParkingSessionById(sessionId, userId, cancellationToken);
        await _garageManagementService.EndParkingSession(session.GarageId, cancellationToken);
        return session.Id;
    }
}
