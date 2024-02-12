using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApI.Controllers;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Services;
using Xunit;

namespace Api.Tests.Controllers;

public class GaragesControllerTest
{
    private readonly GaragesController _controller;
    private readonly IGarageManagementService _service;

    public GaragesControllerTest()
    {
        _service = Substitute.For<IGarageManagementService>();
        _controller = new GaragesController(_service);
    }

    [Fact]
    public async Task GetGarages_ShouldReturnOkWithGarages_WhenCalled()
    {
        _service.GetAllGaragesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Garage>());

        var result = await _controller.GetGarages(new CancellationToken());

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().NotBeNull().And.BeAssignableTo<List<Garage>>();
    }

    [Fact]
    public async Task GetGarage_ShouldReturnOkWithGarage_WhenIdExists()
    {
        var id = Guid.NewGuid();
        var garage = new Garage();

        _service.GetGarageByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(garage);

        var result = await _controller.GetGarage(id, new CancellationToken());

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().NotBeNull().And.BeAssignableTo<Garage>();
    }

    [Fact]
    public async Task GetGarage_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        var id = Guid.NewGuid();

        _service.GetGarageByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((Garage) null);

        var result = await _controller.GetGarage(id, new CancellationToken());

        result.Should().BeOfType<NotFoundResult>();
    }
}
