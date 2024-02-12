using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

public class GarageTest
{
    private const string testName = "Test Garage";
    private static readonly Guid testId = Guid.NewGuid();
    private static readonly List<Door> testDoors = new();

    [Fact]
    public void ParkingAvailable_ShouldReturnTrue_WhenParkingSpotsAvailable()
    {
        // Arrange
        var garage = new Garage
        {
            Id = testId,
            Name = testName,
            Doors = testDoors,
            ParkingSpotsAvailable = 5
        };

        // Act
        var result = garage.ParkingAvailable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ParkingAvailable_ShouldReturnFalse_WhenParkingSpotsUnavailable()
    {
        // Arrange
        var garage = new Garage
        {
            Id = testId,
            Name = testName,
            Doors = testDoors,
            ParkingSpotsAvailable = 0
        };

        // Act
        var result = garage.ParkingAvailable();

        // Assert
        result.Should().BeFalse();
    }


    // these tests aren't SUPER resilient but should likely succeed in any topology.
    [Fact]
    public async Task GarageHardwareReachableAsync_ShouldReturnFalse_WhenAPingFails()
    {
        // Arrange
        var garage = new Garage
        {
            Id = testId, Name = testName, Doors = new List<Door>
            {
                new() {IpAddress = new IpAddress("99.99.99.99")}
            },
            ParkingSpotsAvailable = 5
        };
        // Act
        var result = await garage.GarageHardwareReachableAsync(new CancellationToken());
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GarageHardwareReachableAsync_ShouldReturnTrue_WhenAllPingsSucceed()
    {
        // Arrange
        var garage = new Garage
        {
            Id = testId, Name = testName, Doors = new List<Door>
            {
                new() {IpAddress = new IpAddress("142.250.179.206")}
            },
            ParkingSpotsAvailable = 5
        };
        // Act
        var result = await garage.GarageHardwareReachableAsync(new CancellationToken());
        // Assert
        result.Should().BeTrue();
    }
}
