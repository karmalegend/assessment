using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class UserRepository : IUserRepository
{
    private readonly ParkingDbContext _parkingDbContext;

    public UserRepository(ParkingDbContext parkingDbContext)
    {
        _parkingDbContext = parkingDbContext;
    }

    public async Task<IEnumerable<User>> GetUsersByPartnerIdAsync(string partnerId, CancellationToken cancellationToken)
    {
        return await _parkingDbContext.Users.Where(x => x.PartnerId == partnerId).ToListAsync(cancellationToken);
    }

    public async Task<User> GetUsersByUuidWithParkingSessionsAsync(string userId, CancellationToken cancellationToken)
    {
        return await _parkingDbContext.Users.Where(x => x.Id == new Guid(userId)).Include(x => x.ParkingSessions)
            .FirstAsync(cancellationToken);
    }
}
