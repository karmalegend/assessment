using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ApI.Middleware;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace Api.Tests.Middleware;

public class DomainExceptionHandlingMiddlewareTest
{
    private readonly DomainExceptionHandlingMiddleware _middleware;
    private readonly RequestDelegate _mockRequestDelegate;

    public DomainExceptionHandlingMiddlewareTest()
    {
        _mockRequestDelegate = Substitute.For<RequestDelegate>();
        _middleware = new DomainExceptionHandlingMiddleware(_mockRequestDelegate);
    }

    [Fact]
    public async Task InvokeAsync_WhenCalledWithoutException_ShouldForwardRequest()
    {
        var context = new DefaultHttpContext();

        await _middleware.InvokeAsync(context);

        await _mockRequestDelegate.Received(1).Invoke(context);
    }

    [Fact]
    public async Task InvokeAsync_WhenCalledWithBusinessRuleException_ShouldHandleException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        _mockRequestDelegate.Invoke(context)
            .Returns(Task.FromException(new GarageInteractionFailedException("Exception message")));

        await _middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        body.Should().Contain("Unable to reserve a parking spot at the garageException message");
        context.Response.ContentType.Should().Be("application/json");
        context.Response.StatusCode.Should().Be((int) HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task InvokeAsync_WhenCalledWithNonBusinessRuleException_ShouldNotHandleException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        _mockRequestDelegate.Invoke(context).Returns(Task.FromException(new Exception()));

        var act = async () => await _middleware.InvokeAsync(context);
        await act.Should().ThrowAsync<Exception>();
    }
}
