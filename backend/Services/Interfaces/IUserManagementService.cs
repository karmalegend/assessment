using Domain;

namespace Services;

public interface IUserManagementService
{
    Task<IEnumerable<User>> GetAllUsersByPartnerIdAsync(string partnerId, CancellationToken cancellationToken);
    Task<bool> UserCanStartNewParkingSessionAsync(Guid userId, CancellationToken cancellationToken);
}
