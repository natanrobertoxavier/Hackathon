using Doctor.Api.Controllers.v1;
using Doctor.Application.UseCase.Doctor.ChangePassword;
using Doctor.Application.UseCase.Doctor.Recover.RecoverAll;
using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Application.UseCase.Doctor.Recover.RecoverById;
using Doctor.Application.UseCase.Doctor.Register;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Doctor.Tests.Controllers.v1;

public class DoctorControllerTests
{
    private readonly Mock<IRegisterUseCase> _registerUseCaseMock;
    private readonly Mock<IChangePasswordUseCase> _changePasswordUseCaseMock;
    private readonly Mock<IRecoverAllUseCase> _recoverAllUseCaseMock;
    private readonly Mock<IRecoverByCRUseCase> _recoverByCRUseCaseMock;
    private readonly Mock<IRecoverByIdUseCase> _recoverByIdUseCaseMock;
    private readonly DoctorController _controller;

    public DoctorControllerTests()
    {
        _registerUseCaseMock = new Mock<IRegisterUseCase>();
        _changePasswordUseCaseMock = new Mock<IChangePasswordUseCase>();
        _recoverAllUseCaseMock = new Mock<IRecoverAllUseCase>();
        _recoverByCRUseCaseMock = new Mock<IRecoverByCRUseCase>();
        _recoverByIdUseCaseMock = new Mock<IRecoverByIdUseCase>();
        _controller = new DoctorController();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnCreated_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RequestRegisterDoctor("Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());
        var response = new Result<MessageResult>();
        response.Succeeded(new MessageResult("Doctor registered successfully"));

        _registerUseCaseMock
            .Setup(x => x.RegisterDoctorAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RegisterAsync(_registerUseCaseMock.Object, request);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnOk_WhenPasswordChangeIsSuccessful()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");
        var response = new Result<MessageResult>();
        response.Succeeded(new MessageResult("Password changed successfully"));

        _changePasswordUseCaseMock
            .Setup(x => x.ChangePasswordAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ChangePasswordAsync(_changePasswordUseCaseMock.Object, request);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnOk_WhenDoctorsAreFound()
    {
        // Arrange
        var response = new Result<IEnumerable<ResponseDoctor>>();
        response.Succeeded(new List<ResponseDoctor>
        {
            new ResponseDoctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "email@example.com", "12345", null, null)
        });

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
    public async Task RecoverByCRAsync_ShouldReturnOk_WhenDoctorIsFound()
    {
        // Arrange
        var cr = "12345";
        var response = new Result<ResponseDoctor>();
        var doctor = new ResponseDoctor
        {
            DoctorId = Guid.NewGuid(),
            RegistrationDate = DateTime.UtcNow,
            Name = "Doctor Name",
            Email = "email@example.com",
            CR = cr,
            SpecialtyDoctor = null
        };
        response.Succeeded(doctor);

        _recoverByCRUseCaseMock
            .Setup(x => x.RecoverByCRAsync(cr))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RecoverByCRAsync(_recoverByCRUseCaseMock.Object, cr);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnOk_WhenDoctorIsFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new Result<ResponseDoctor>();
        var doctor = new ResponseDoctor
        {
            DoctorId = id,
            RegistrationDate = DateTime.UtcNow,
            Name = "Doctor Name",
            Email = "email@example.com",
            CR = "12345",
            SpecialtyDoctor = null
        };
        response.Succeeded(doctor);

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
}
