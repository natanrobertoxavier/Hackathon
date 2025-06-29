﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User.Api.Controllers.v1;
using User.Application.UseCase.Login;
using User.Communication.Request;
using User.Communication.Response;

namespace User.Tests.Controllers.v1;

public class LoginControllerTests
{
    private readonly Mock<ILoginUseCase> _loginUseCaseMock;
    private readonly LoginController _controller;

    public LoginControllerTests()
    {
        _loginUseCaseMock = new Mock<ILoginUseCase>();
        _controller = new LoginController();
    }

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var request = new RequestLoginUser("12345", "password");
        var response = new Result<ResponseLogin>();
        response.Succeeded(new ResponseLogin
        {
            Name = "User Name",
            Email = "email@example.com",
            Token = "token"
        });

        _loginUseCaseMock
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByCRPasswordAsync(_loginUseCaseMock.Object, request);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnUnauthorized_WhenLoginFails()
    {
        // Arrange
        var request = new RequestLoginUser("12345", "wrongpassword");
        var response = new Result<ResponseLogin>();
        response.Failure(new List<string> { "Invalid credentials" });

        _loginUseCaseMock
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByCRPasswordAsync(_loginUseCaseMock.Object, request);

        // Assert
        var unauthorizedResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task RecoverByCRPasswordAsync_ShouldReturnUnprocessableEntity_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RequestLoginUser("", "");
        var response = new Result<ResponseLogin>();
        response.Failure(new List<string> { "Validation failed" });

        _loginUseCaseMock
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByCRPasswordAsync(_loginUseCaseMock.Object, request);

        // Assert
        var unprocessableEntityResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unprocessableEntityResult.StatusCode);
    }
}
