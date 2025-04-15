using Consultation.Api.Controllers.v1;
using Consultation.Application.UseCase.Consultation.Register;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Consultation.Tests.Controllers.v1;

public class ConsultationControllerTests
{
    private readonly Mock<IRegisterUseCase> _mockRegisterUseCase;
    private readonly ConsultationController _controller;

    public ConsultationControllerTests()
    {
        _mockRegisterUseCase = new Mock<IRegisterUseCase>();
        _controller = new ConsultationController();
    }

    [Fact]
    public async Task RegisterConsultationAsync_ShouldReturn201Created_WhenUseCaseSucceeds()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.UtcNow
        };

        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

        _mockRegisterUseCase
            .Setup(x => x.RegisterConsultationAsync(request))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RegisterConsultationAsync(_mockRegisterUseCase.Object, request) as ObjectResult; ;

        // Assert
        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(result, response.Value);
    }

    [Fact]
    public async Task RegisterConsultationAsync_ShouldReturn400BadRequest_WhenUseCaseFails()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.UtcNow
        };

        var result = new Result<MessageResult>();
        result.Failure(new List<string>() { "Validation failed" });

        _mockRegisterUseCase
            .Setup(x => x.RegisterConsultationAsync(request))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RegisterConsultationAsync(_mockRegisterUseCase.Object, request);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.False(((Result<MessageResult>)badRequestResult.Value).Success);
    }
}
