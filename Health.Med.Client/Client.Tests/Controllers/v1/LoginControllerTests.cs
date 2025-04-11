using Client.Api.Controllers.v1;
using Client.Application.UseCase.Login;
using Client.Communication.Request;
using Client.Communication.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Client.Tests.Controllers.v1;

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
    public async Task RecoverByEmailPasswordAsync_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var request = new RequestLoginClient("any@email.com", "password");
        var response = new Result<ResponseLogin>();
        response.Succeeded(new ResponseLogin
        {
            Name = "Client Name",
            Email = "email@example.com",
            Token = "token"
        });

        _loginUseCaseMock
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByEmailPasswordAsync(_loginUseCaseMock.Object, request);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RecoverByEmailPasswordAsync_ShouldReturnUnauthorized_WhenLoginFails()
    {
        // Arrange
        var request = new RequestLoginClient("any@email.com", "wrongpassword");
        var response = new Result<ResponseLogin>();
        response.Failure(new List<string> { "Invalid credentials" });

        _loginUseCaseMock
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByEmailPasswordAsync(_loginUseCaseMock.Object, request);

        // Assert
        var unauthorizedResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task RecoverByEmailPasswordAsync_ShouldReturnUnprocessableEntity_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RequestLoginClient("", "");
        var response = new Result<ResponseLogin>();
        response.Failure(new List<string> { "Validation failed" });

        _loginUseCaseMock
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByEmailPasswordAsync(_loginUseCaseMock.Object, request);

        // Assert
        var unprocessableEntityResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unprocessableEntityResult.StatusCode);
    }
}
