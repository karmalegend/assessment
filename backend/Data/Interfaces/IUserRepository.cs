using Domain;

namespace Data;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersByPartnerIdAsync(string partnerId, CancellationToken cancellationToken);
    Task<User> GetUsersByUuidWithParkingSessionsAsync(string userId, CancellationToken cancellationToken);
}
