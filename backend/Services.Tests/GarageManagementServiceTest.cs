using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Services.Tests;

public class GarageManagementServiceTest
{
    private readonly IGarageRepository _garageRepository;
    private readonly GarageManagementService _sut;

    public GarageManagementServiceTest()
    {
        _garageRepository = Substitute.For<IGarageRepository>();
        _sut = new GarageManagementService(_garageRepository);
    }

    [Fact]
    public async Task CanAcceptParkingSessions_ShouldReturnTrue_WhenSpotsAvailableAndHardwareReachable()
    {
        var garage = new Garage
        {
            ParkingSpotsAvailable = 5,
            Id = Guid.Parse("7ae45434-e26c-4fe5-8984-04d399e627c6"),
            Doors = new List<Door>()
        };
        _garageRepository.GetGarageByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(garage);

        var canAccept = await _sut.CanAcceptParkingSessions(garage.Id, default);

        canAccept.Should().BeTrue();

        await _garageRepository.Received()
            .GetGarageByIdAsync(Arg.Is<Guid>(x => x == Guid.Parse("7ae45434-e26c-4fe5-8984-04d399e627c6")),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public void CanAcceptParkingSessions_ShouldThrowException_WhenGarageIsNull()
    {
        _garageRepository.GetGarageByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Garage) null!);

        Func<Task> action = async () => await _sut.CanAcceptParkingSessions(Guid.NewGuid(), default);

        action.Should().ThrowAsync<ResourceNotFoundExceptionException>();
    }

    [Fact]
    public async Task CanAcceptParkingSessions_ShouldReturnFalse_WhenSpotsUnavailable()
    {
        var garage = new Garage
        {
            ParkingSpotsAvailable = 0,
            Id = Guid.Parse("7ae45434-e26c-4fe5-8984-04d399e627c6"),
            Doors = new List<Door>()
        };
        _garageRepository.GetGarageByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(garage);

        var canAccept = await _sut.CanAcceptParkingSessions(garage.Id, default);

        canAccept.Should().BeFalse();

        await _garageRepository.Received()
            .GetGarageByIdAsync(Arg.Is<Guid>(x => x == Guid.Parse("7ae45434-e26c-4fe5-8984-04d399e627c6")),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReserveParkingSpot_ShouldCallRepository_WhenCalled()
    {
        var guid = Guid.Parse("7ae45434-e26c-4fe5-8984-04d399e627c6");

        await _sut.ReserveParkingSpot(guid, default);

        await _garageRepository.Received(1).DecrementParkingCapacity(guid, default);
    }

    [Fact]
    public async Task EndParkingSession_ShouldIncrementParkingCapacity_WhenCalled()
    {
        var guid = Guid.Parse("7ae45434-e26c-4fe5-8984-04d399e627c6");

        _garageRepository.GetGarageByIdAsync(guid, default).Returns(new Garage
        {
            Doors = new List<Door> {new() {DoorType = DoorType.Exit, IpAddress = new IpAddress("99.99.99.99")}}
        });

        await _sut.EndParkingSession(guid, default);
        await _garageRepository.Received(1).IncrementParkingCapacity(guid, default);
    }
}
