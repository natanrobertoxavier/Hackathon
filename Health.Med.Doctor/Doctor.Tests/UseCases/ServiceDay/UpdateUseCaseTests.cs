using Doctor.Application.UseCase.ServiceDay.Delete;
using Doctor.Application.UseCase.ServiceDay.Register;
using Doctor.Application.UseCase.ServiceDay.Update;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Health.Med.Exceptions.ExceptionBase;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.ServiceDay;

public class UpdateUseCaseTests
{
    private readonly Mock<IDeleteUseCase> _deleteUseCaseMock;
    private readonly Mock<IRegisterUseCase> _registerUseCaseMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly UpdateUseCase _updateUseCase;

    public UpdateUseCaseTests()
    {
        _deleteUseCaseMock = new Mock<IDeleteUseCase>();
        _registerUseCaseMock = new Mock<IRegisterUseCase>();
        _loggerMock = new Mock<ILogger>();

        _updateUseCase = new UpdateUseCase(
            _deleteUseCaseMock.Object,
            _registerUseCaseMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task UpdateServiceDayAsync_ShouldReturnSuccess_WhenAllStepsSucceed()
    {
        // Arrange
        var request = new RequestServiceDay(new List<Communication.Request.ServiceDay>
        {
            new Communication.Request.ServiceDay{ Day = 0, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
        });

        var successResult = new Result<MessageResult>();
        successResult.Succeeded(new MessageResult("Successful"));

        _deleteUseCaseMock
            .Setup(x => x.DeleteServiceDayAsync(It.IsAny<RequestDeleteServiceDay>()))
            .ReturnsAsync(successResult);

        _registerUseCaseMock
            .Setup(x => x.RegisterServiceDayAsync(It.IsAny<RequestServiceDay>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _updateUseCase.UpdateServiceDayAsync(request);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.Equal("Atualização realizada com sucesso", result.GetData().Message);
    }

    [Fact]
    public async Task UpdateServiceDayAsync_ShouldThrowValidationErrorsException_WhenValidationFails()
    {
        // Arrange
        var request = new RequestServiceDay(new List<Communication.Request.ServiceDay>
        {
            new Communication.Request.ServiceDay{ Day = 0, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(9) }
        });

        // Act
        var result = await _updateUseCase.UpdateServiceDayAsync(request);

        // Assert
        Assert.False(result.IsSuccess());
        Assert.Contains("O horário de início deve ser menor que o horário de fim.", result.Errors);
    }

    [Fact]
    public async Task UpdateServiceDayAsync_ShouldFail_WhenDeleteServiceDayFails()
    {
        // Arrange
        var request = new RequestServiceDay(new List<Communication.Request.ServiceDay>
        {
            new Communication.Request.ServiceDay { Day = 0, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
        });

        var failResult = new Result<MessageResult>();
        failResult.Failure(new List<string>() { "Fail" });

        _deleteUseCaseMock
            .Setup(x => x.DeleteServiceDayAsync(It.IsAny<RequestDeleteServiceDay>()))
            .ReturnsAsync(failResult);

        // Act
        var result = await _updateUseCase.UpdateServiceDayAsync(request);

        // Assert
        Assert.False(result.IsSuccess());
        Assert.Contains("Erro ao remover os dias existentes", result.Errors.First());
    }

    [Fact]
    public async Task UpdateServiceDayAsync_ShouldFail_WhenRegisterServiceDayFails()
    {
        // Arrange
        var request = new RequestServiceDay(new List<Communication.Request.ServiceDay>
        {
            new Communication.Request.ServiceDay { Day = 0, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17) }
        });

        var successResult = new Result<MessageResult>();
        successResult.Succeeded(new MessageResult("Successful"));

        _deleteUseCaseMock
            .Setup(x => x.DeleteServiceDayAsync(It.IsAny<RequestDeleteServiceDay>()))
            .ReturnsAsync(successResult);

        var failResult = new Result<MessageResult>();
        failResult.Failure(new List<string>() { "Fail" });

        _registerUseCaseMock
            .Setup(x => x.RegisterServiceDayAsync(It.IsAny<RequestServiceDay>()))
            .ReturnsAsync(failResult);

        // Act
        var result = await _updateUseCase.UpdateServiceDayAsync(request);

        // Assert
        Assert.False(result.IsSuccess());
        Assert.Contains("Erro ao incluir os novos dias", result.Errors.First());
    }
}
