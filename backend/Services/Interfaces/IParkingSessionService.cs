namespace Services;

public interface IParkingSessionService
{
    Task<Guid> StartParkingSession(Guid garageId, Guid userId, CancellationToken cancellationToken);
    Task<Guid> StopParkingSession(Guid sessionId, Guid userId, CancellationToken cancellationToken);
}
