using System.Data;
using Domain;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class GarageRepository : IGarageRepository
{
    private readonly ParkingDbContext _parkingDbContext;

    public GarageRepository(ParkingDbContext parkingDbContext)
    {
        _parkingDbContext = parkingDbContext;
    }

    public async Task<IEnumerable<Garage>> GetGaragesAsync(CancellationToken cancellationToken)
    {
        return await _parkingDbContext.Garages.Include(x => x.Doors).ToListAsync(cancellationToken);
    }

    public async Task<Garage?> GetGarageByIdAsync(Guid garageId, CancellationToken cancellationToken)
    {
        return await _parkingDbContext.Garages
            .Include(x => x.Doors)
            .Where(x => x.Id == garageId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    ///     Decrements the parking capacity of a garage by 1.
    ///     Only call this when verified the garage exists.
    /// </summary>
    /// <param name="garageId">The ID of the garage.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task DecrementParkingCapacity(Guid garageId, CancellationToken cancellationToken)
    {
        try
        {
            var garage = await _parkingDbContext.Garages.Where(x => x.Id == garageId).FirstAsync(cancellationToken);
            if (garage.ParkingSpotsAvailable < 1) throw new GarageInteractionFailedException();

            garage.ParkingSpotsAvailable -= 1;
            await UpdateGarage(cancellationToken);
        }
        catch (DBConcurrencyException)
        {
            await DecrementParkingCapacity(garageId, cancellationToken);
        }
    }

    public async Task IncrementParkingCapacity(Guid garageId, CancellationToken cancellationToken)
    {
        try
        {
            var garage = await _parkingDbContext.Garages.Where(x => x.Id == garageId).FirstAsync(cancellationToken);
            if (garage.ParkingSpotsAvailable < 1) throw new GarageInteractionFailedException();

            garage.ParkingSpotsAvailable += 1;
            await UpdateGarage(cancellationToken);
        }
        catch (DBConcurrencyException)
        {
            await DecrementParkingCapacity(garageId, cancellationToken);
        }
    }

    private async Task UpdateGarage(CancellationToken cancellationToken)
    {
        await _parkingDbContext.SaveChangesAsync(cancellationToken);
    }
}
