using Domain;

namespace Data;

public interface IGarageRepository
{
    Task<IEnumerable<Garage>> GetGaragesAsync(CancellationToken cancellationToken);
    Task<Garage?> GetGarageByIdAsync(Guid garageId, CancellationToken cancellationToken);
    Task DecrementParkingCapacity(Guid garageId, CancellationToken cancellationToken);
    Task IncrementParkingCapacity(Guid garageId, CancellationToken cancellationToken);
}
