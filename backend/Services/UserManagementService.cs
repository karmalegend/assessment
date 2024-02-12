using Data;
using Domain;

namespace Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> UserCanStartNewParkingSessionAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUsersByUuidWithParkingSessionsAsync(userId.ToString(), cancellationToken);
        return !user.HasActiveParkingSessions();
    }

    public async Task<IEnumerable<User>> GetAllUsersByPartnerIdAsync(string partnerId,
        CancellationToken cancellationToken)
    {
        return await _userRepository.GetUsersByPartnerIdAsync(partnerId, cancellationToken);
    }
}
