using Domain;

namespace Services;

public interface IGarageManagementService
{
    public Task<bool> CanAcceptParkingSessions(Guid garageId, CancellationToken cancellationToken);
    public Task<IEnumerable<Garage>> GetAllGaragesAsync(CancellationToken cancellationToken);
    Task<Garage?> GetGarageByIdAsync(Guid garageId, CancellationToken cancellationToken);
    Task ReserveParkingSpot(Guid garageId, CancellationToken cancellationToken);
    Task OpenEntryDoor(Guid garageId, CancellationToken cancellationToken);
    Task EndParkingSession(Guid garageId, CancellationToken cancellationToken);
}
