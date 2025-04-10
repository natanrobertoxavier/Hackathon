using User.Api.Controllers.v1;
using User.Application.UseCase.ChangePassword;
using User.Application.UseCase.Recover.RecoverAll;
using User.Application.UseCase.Recover.RecoverByEmail;
using User.Application.UseCase.Register;
using User.Communication.Request;
using User.Communication.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace User.Tests.Controllers.v1;

public class UserControllerTests
{
    private readonly Mock<IRegisterUseCase> _registerUseCaseMock;
    private readonly Mock<IChangePasswordUseCase> _changePasswordUseCaseMock;
    private readonly Mock<IRecoverAllUseCase> _recoverAllUseCaseMock;
    private readonly Mock<IRecoverByEmailUseCase> _recoverByEmailUseCaseMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _registerUseCaseMock = new Mock<IRegisterUseCase>();
        _changePasswordUseCaseMock = new Mock<IChangePasswordUseCase>();
        _recoverAllUseCaseMock = new Mock<IRecoverAllUseCase>();
        _recoverByEmailUseCaseMock = new Mock<IRecoverByEmailUseCase>();
        _controller = new UserController();
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturnCreated_WhenSuccess()
    {
        // Arrange
        var request = new RequestRegisterUser("John Doe", "john.doe@example.com", "password123");
        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("User registered successfully"));

        _registerUseCaseMock.Setup(x => x.RegisterUserAsync(request)).ReturnsAsync(result);

        // Act
        var response = await _controller.RegisterUserAsync(_registerUseCaseMock.Object, request) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");
        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("Password changed successfully"));

        _changePasswordUseCaseMock.Setup(x => x.ChangePasswordAsync(request)).ReturnsAsync(result);

        // Act
        var response = await _controller.ChangePasswordAsync(_changePasswordUseCaseMock.Object, request) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var result = new Result<IEnumerable<ResponseUser>>();
        result.Succeeded(new List<ResponseUser>
        {
            new ResponseUser { Id = Guid.NewGuid(), Name = "John Doe", Email = "john.doe@example.com", RegistrationDate = DateTime.UtcNow }
        });

        _recoverAllUseCaseMock.Setup(x => x.RecoverAllAsync(1, 5)).ReturnsAsync(result);

        // Act
        var response = await _controller.RecoverAllAsync(_recoverAllUseCaseMock.Object, 1, 5) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var email = "john.doe@example.com";

        var result = new Result<ResponseUser>();
        result.Succeeded(new ResponseUser { Id = Guid.NewGuid(), Name = "John Doe", Email = "john.doe@example.com", RegistrationDate = DateTime.UtcNow });
        
        _recoverByEmailUseCaseMock.Setup(x => x.RecoverByEmailAsync(email)).ReturnsAsync(result);

        // Act
        var response = await _controller.RecoverByEmailAsync(_recoverByEmailUseCaseMock.Object, email) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }
}

