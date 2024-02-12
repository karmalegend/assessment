using Domain;

namespace Data;

public interface IParkingSessionRepository
{
    Task<ParkingSession> CreateParkingSessions(ParkingSession parkingSession, CancellationToken cancellationToken);
    Task<ParkingSession> StopParkingSessionById(Guid sessionId, Guid userId, CancellationToken cancellationToken);
}
