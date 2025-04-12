using Doctor.Api.Controllers.v1;
using Doctor.Application.UseCase.ServiceDay.Delete;
using Doctor.Application.UseCase.ServiceDay.Register;
using Doctor.Application.UseCase.ServiceDay.Update;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Doctor.Tests.Controllers;

public class ServiceDayControllerTests
{
    private readonly Mock<IRegisterUseCase> _registerUseCaseMock;
    private readonly Mock<IUpdateUseCase> _updateUseCaseMock;
    private readonly Mock<IDeleteUseCase> _deleteUseCaseMock;
    private readonly ServiceDayController _controller;

    public ServiceDayControllerTests()
    {
        _registerUseCaseMock = new Mock<IRegisterUseCase>();
        _updateUseCaseMock = new Mock<IUpdateUseCase>();
        _deleteUseCaseMock = new Mock<IDeleteUseCase>();

        _controller = new ServiceDayController();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnCreated_WhenUseCaseSucceeds()
    {
        // Arrange
        var request = new RequestServiceDay(new List<ServiceDay>
        {
            new ServiceDay { Day = 0, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
        });

        var successResult = new Result<MessageResult>();
        successResult.Succeeded(new MessageResult("Successful"));

        _registerUseCaseMock
            .Setup(x => x.RegisterServiceDayAsync(request))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.RegisterAsync(_registerUseCaseMock.Object, request); ;

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<Result<MessageResult>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Successful", response.Data.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnBadRequest_WhenUseCaseFails()
    {
        // Arrange
        var request = new RequestServiceDay(new List<ServiceDay>
        {
            new ServiceDay { Day = 0, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
        });

        var failResult = new Result<MessageResult>();
        failResult.Failure(new List<string>() { "Fail" });

        _registerUseCaseMock
            .Setup(x => x.RegisterServiceDayAsync(request))
            .ReturnsAsync(failResult);

        // Act
        var result = await _controller.RegisterAsync(_registerUseCaseMock.Object, request);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<Result<MessageResult>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Fail", response.Errors);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnCreated_WhenUseCaseSucceeds()
    {
        // Arrange
        var request = new RequestServiceDay(new List<ServiceDay>
        {
            new ServiceDay { Day = 0, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
        });

        var successResult = new Result<MessageResult>();
        successResult.Succeeded(new MessageResult("Successful"));

        _updateUseCaseMock
            .Setup(x => x.UpdateServiceDayAsync(request))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.DeleteAsync(_updateUseCaseMock.Object, request);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<Result<MessageResult>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Successful", response.Data.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnCreated_WhenUseCaseSucceeds()
    {
        // Arrange
        var request = new RequestDeleteServiceDay(new List<DaysToRemove>
        {
            new DaysToRemove(DayOfWeek.Monday)
        });

        var successResult = new Result<MessageResult>();
        successResult.Succeeded(new MessageResult("Successful"));

        _deleteUseCaseMock
            .Setup(x => x.DeleteServiceDayAsync(request))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.DeleteAsync(_deleteUseCaseMock.Object, request);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<Result<MessageResult>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Successful", response.Data.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnBadRequest_WhenUseCaseFails()
    {
        // Arrange
        var request = new RequestDeleteServiceDay(new List<DaysToRemove>
        {
            new DaysToRemove(DayOfWeek.Monday)
        });

        var failResult = new Result<MessageResult>();
        failResult.Failure(new List<string>() { "Fail" });

        _deleteUseCaseMock
            .Setup(x => x.DeleteServiceDayAsync(request))
            .ReturnsAsync(failResult);

        // Act
        var result = await _controller.DeleteAsync(_deleteUseCaseMock.Object, request);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<Result<MessageResult>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Fail", response.Errors);
    }
}
