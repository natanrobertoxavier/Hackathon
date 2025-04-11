using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Client.Api.Controllers.v1;
using Client.Application.UseCase.ChangePassword;
using Client.Application.UseCase.Recover.RecoverAll;
using Client.Application.UseCase.Recover.RecoverByCPF;
using Client.Application.UseCase.Recover.RecoverByEmail;
using Client.Application.UseCase.Register;
using Client.Communication.Request;
using Client.Communication.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Client.Tests.Controllers.v1;

public class ClientControllerTests
{
    [Fact]
    public async Task RegisterUserAsync_ShouldReturnCreated_WhenUseCaseSucceeds()
    {
        // Arrange
        var mockUseCase = new Mock<IRegisterUseCase>();
        var request = new RequestRegisterClient(
            "John Doe",
            "John",
            "john.doe@example.com",
            "12345678900",
            "password123");

        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("Client registered successfully"));

        mockUseCase.Setup(x => x.RegisterClientAsync(request)).ReturnsAsync(result);

        var controller = new ClientController();

        // Act
        var response = await controller.RegisterUserAsync(mockUseCase.Object, request) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var mockUseCase = new Mock<IChangePasswordUseCase>();
        var request = new RequestChangePassword(
            "oldPassword123",
            "newPassword123");

        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("Password changed successfully"));

        mockUseCase.Setup(x => x.ChangePasswordAsync(request)).ReturnsAsync(result);

        var controller = new ClientController();

        // Act
        var response = await controller.ChangePasswordAsync(mockUseCase.Object, request) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverAllUseCase>();

        var result = new Result<IEnumerable<ResponseClient>>();
        result.Succeeded(new List<ResponseClient>
        {
            new ResponseClient(Guid.NewGuid(), DateTime.UtcNow, "John Doe", "john.doe@example.com", "12345678900")
        });

        mockUseCase.Setup(x => x.RecoverAllAsync(1, 5)).ReturnsAsync(result);

        var controller = new ClientController();

        // Act
        var response = await controller.RecoverAllAsync(mockUseCase.Object, 1, 5) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverByEmailUseCase>();
        var email = "john.doe@example.com";
        var result = new Result<ResponseClient>();
        result.Succeeded(new ResponseClient(Guid.NewGuid(), DateTime.UtcNow, "John Doe", "john.doe@example.com", "12345678900"));

        mockUseCase.Setup(x => x.RecoverByEmailAsync(email)).ReturnsAsync(result);

        var controller = new ClientController();

        // Act
        var response = await controller.RecoverByEmailAsync(mockUseCase.Object, email) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task RecoverByCPFAsync_ShouldReturnOk_WhenUseCaseSucceeds()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverByCPFUseCase>();
        var cpf = "12345678900";
        var result = new Result<ResponseClient>();
        result.Succeeded(new ResponseClient(Guid.NewGuid(), DateTime.UtcNow, "John Doe", "john.doe@example.com", "12345678900"));

        mockUseCase.Setup(x => x.RecoverByCPFAsync(cpf)).ReturnsAsync(result);

        var controller = new ClientController();

        // Act
        var response = await controller.RecoverByCPFAsync(mockUseCase.Object, cpf) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(result, response.Value);
    }
}
