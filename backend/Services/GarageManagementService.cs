using Data;
using Domain;
using Domain.Exceptions;

namespace Services;

public class GarageManagementService : IGarageManagementService
{
    private readonly IGarageRepository _garageRepository;


    public GarageManagementService(IGarageRepository garageRepository)
    {
        _garageRepository = garageRepository;
    }

    public async Task<bool> CanAcceptParkingSessions(Guid garageId, CancellationToken cancellationToken)
    {
        var garage = await _garageRepository.GetGarageByIdAsync(garageId, cancellationToken);

        if (garage == null) throw new ResourceNotFoundExceptionException();

        return garage.ParkingAvailable() && await garage.GarageHardwareReachableAsync(cancellationToken);
    }

    public async Task ReserveParkingSpot(Guid garageId, CancellationToken cancellationToken)
    {
        await _garageRepository.DecrementParkingCapacity(garageId, cancellationToken);
    }

    public async Task OpenEntryDoor(Guid garageId, CancellationToken cancellationToken)
    {
        var garage = await _garageRepository.GetGarageByIdAsync(garageId, cancellationToken);

        await garage!.OpenEntryDoorAsync();
    }

    public async Task EndParkingSession(Guid garageId, CancellationToken cancellationToken)
    {
        var garage = await _garageRepository.GetGarageByIdAsync(garageId, cancellationToken);
        await garage!.OpenExitDoorAsync();
        await _garageRepository.IncrementParkingCapacity(garageId, cancellationToken);
    }

    public async Task<IEnumerable<Garage>> GetAllGaragesAsync(CancellationToken cancellationToken)
    {
        return await _garageRepository.GetGaragesAsync(cancellationToken);
    }

    public async Task<Garage?> GetGarageByIdAsync(Guid garageId, CancellationToken cancellationToken)
    {
        return await _garageRepository.GetGarageByIdAsync(garageId, cancellationToken);
    }
}
