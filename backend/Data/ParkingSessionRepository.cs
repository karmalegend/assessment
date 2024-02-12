using Domain;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ParkingSessionRepository : IParkingSessionRepository
{
    private readonly ParkingDbContext _parkingDbContext;

    public ParkingSessionRepository(ParkingDbContext parkingDbContext)
    {
        _parkingDbContext = parkingDbContext;
    }

    public async Task<ParkingSession> CreateParkingSessions(ParkingSession parkingSession,
        CancellationToken cancellationToken)
    {
        await _parkingDbContext.ParkingSessions.AddAsync(parkingSession, cancellationToken);
        await _parkingDbContext.SaveChangesAsync(cancellationToken);
        return parkingSession;
    }

    public async Task<ParkingSession> StopParkingSessionById(Guid sessionId, Guid userId,
        CancellationToken cancellationToken)
    {
        var session = await _parkingDbContext.ParkingSessions.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.SessionsState == ParkingSessionsState.InProgress && x.Id == sessionId,
            cancellationToken);

        if (session == null)
        {
            throw new ResourceNotFoundExceptionException(" Parking session not found");
        }

        session.SessionsState = ParkingSessionsState.Ended;
        session.EndTime = DateTime.UtcNow;

        await _parkingDbContext.SaveChangesAsync(cancellationToken);

        return session;
    }
}
