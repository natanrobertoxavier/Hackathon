using Doctor.Api.Controllers.v1;
using Doctor.Application.UseCase.Specialty.Recover.RecoverAll;
using Doctor.Application.UseCase.Specialty.Recover.RecoverById;
using Doctor.Application.UseCase.Specialty.Register;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Doctor.Tests.Controllers.v1;

public class SpecialtyControllerTests
{
    private readonly Mock<IRegisterUseCase> _registerUseCaseMock;
    private readonly Mock<IRecoverAllUseCase> _recoverAllUseCaseMock;
    private readonly Mock<IRecoverByIdUseCase> _recoverByIdUseCaseMock;
    private readonly SpecialtyController _controller;

    public SpecialtyControllerTests()
    {
        _registerUseCaseMock = new Mock<IRegisterUseCase>();
        _recoverAllUseCaseMock = new Mock<IRecoverAllUseCase>();
        _recoverByIdUseCaseMock = new Mock<IRecoverByIdUseCase>();
        _controller = new SpecialtyController();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnCreated_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RequestRegisterSpecialty("Cardiology");
        var response = new Result<MessageResult>();
        response.Succeeded(new MessageResult("Specialty registered successfully"));

        _registerUseCaseMock
            .Setup(x => x.RegisterSpecialtyAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RegisterAsync(_registerUseCaseMock.Object, request);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnOk_WhenSpecialtiesAreFound()
    {
        // Arrange
        var specialties = new List<ResponseSpecialty>
        {
            new ResponseSpecialty(Guid.NewGuid(), DateTime.UtcNow, "Cardiology", "Standard Description")
        };

        var response = new Result<IEnumerable<ResponseSpecialty>>();
        response.Succeeded(specialties);

        _recoverAllUseCaseMock
            .Setup(x => x.RecoverAllAsync(1, 5))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverAllAsync(_recoverAllUseCaseMock.Object, 1, 5);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnOk_WhenSpecialtyIsFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new Result<ResponseSpecialty>();
        response.Succeeded(new ResponseSpecialty
        {
            SpecialtyId = id,
            RegistrationDate = DateTime.UtcNow,
            Description = "Cardiology",
            StandardDescription = "Standard Description"
        });

        _recoverByIdUseCaseMock
            .Setup(x => x.RecoverByIdAsync(id))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByIdAsync(_recoverByIdUseCaseMock.Object, id);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnNotFound_WhenSpecialtyIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new Result<ResponseSpecialty>();
        response.Failure(new List<string> { "Specialty not found" });

        _recoverByIdUseCaseMock
            .Setup(x => x.RecoverByIdAsync(id))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByIdAsync(_recoverByIdUseCaseMock.Object, id);

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, notFoundResult.StatusCode);
    }
}
