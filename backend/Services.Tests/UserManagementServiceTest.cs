using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Domain;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Services.Tests;

public class UserManagementServiceTest
{
    private readonly CancellationToken _cancellationToken;
    private readonly UserManagementService _userManagementService;
    private readonly IUserRepository _userRepository;

    public UserManagementServiceTest()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userManagementService = new UserManagementService(_userRepository);
        _cancellationToken = new CancellationToken();
    }

    [Fact]
    public async Task GetAllUsersByPartnerIdAsync_ShouldReturnsEmptyUserList_WhenNoUsersAreFound()
    {
        _userRepository.GetUsersByPartnerIdAsync("nonexistentPartnerId", _cancellationToken).Returns(new List<User>());
        var users = await _userManagementService.GetAllUsersByPartnerIdAsync("nonexistentPartnerId",
            _cancellationToken);
        users.Should().BeEmpty();
    }

    // More test cases for GetAllUsersByPartnerIdAsync could be added here

    [Fact]
    public async Task UserCanStartNewParkingSessionAsync_ShouldReturnsFalse_WhenUserHasActiveSessions()
    {
        var user = new User
        {
            ParkingSessions = new List<ParkingSession>
            {
                new() {SessionsState = ParkingSessionsState.InProgress}
            }
        };

        _userRepository.GetUsersByUuidWithParkingSessionsAsync(Arg.Any<string>(), _cancellationToken).Returns(user);
        var result =
            await _userManagementService.UserCanStartNewParkingSessionAsync(Guid.NewGuid(), _cancellationToken);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserCanStartNewParkingSessionAsync_ShouldReturnsTrue_WhenUserDoesNotHaveActiveSessions()
    {
        var user = new User
        {
            ParkingSessions = new List<ParkingSession>
            {
                new() {SessionsState = ParkingSessionsState.Ended}
            }
        };

        _userRepository.GetUsersByUuidWithParkingSessionsAsync(Arg.Any<string>(), _cancellationToken).Returns(user);
        var result =
            await _userManagementService.UserCanStartNewParkingSessionAsync(Guid.NewGuid(), _cancellationToken);
        result.Should().BeTrue();
    }
}
