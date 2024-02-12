using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ApI.Controllers;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Services;
using Xunit;

namespace Api.Tests.Controllers;

public class UsersControllerTest
{
    private const string PartnerId = "testPartnerId";
    private const string UserIdClaimType = "partnerId";
    private readonly IUserManagementService _userManagementServiceMock;
    private readonly UsersController _usersController;

    public UsersControllerTest()
    {
        _userManagementServiceMock = Substitute.For<IUserManagementService>();
        _usersController = new UsersController(_userManagementServiceMock);

        // Mock user claims
        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(UserIdClaimType, PartnerId)
                }))
            }
        };
        _usersController.ControllerContext = controllerContext;
    }

    [Fact]
    public async Task GetUsers_ValidPartnerId_ReturnsUsers()
    {
        var testUsers = new List<User>
        {
            new() {PartnerId = PartnerId, Id = Guid.NewGuid()},
            new() {PartnerId = PartnerId, Id = Guid.NewGuid()},
            new() {PartnerId = PartnerId, Id = Guid.NewGuid()}
        };

        _userManagementServiceMock
            .GetAllUsersByPartnerIdAsync(PartnerId, Arg.Any<CancellationToken>())
            .Returns(testUsers);

        var users = await _usersController.GetUsers(CancellationToken.None);

        users.Should().BeEquivalentTo(testUsers);
    }

    [Fact]
    public async Task GetUsers_InvalidPartnerId_ReturnsEmpty()
    {
        _userManagementServiceMock
            .GetAllUsersByPartnerIdAsync(PartnerId, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        var users = await _usersController.GetUsers(CancellationToken.None);

        users.Should().BeEmpty();
    }
}
