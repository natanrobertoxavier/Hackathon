using Doctor.Api.Filters;
using Doctor.Application.UseCase.Doctor.ChangePassword;
using Doctor.Application.UseCase.Doctor.Recover.RecoverAll;
using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Application.UseCase.Doctor.Recover.RecoverById;
using Doctor.Application.UseCase.Doctor.Register;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Api.Controllers.v1;

public class DoctorController : HealthMedController
{
    [HttpPost]
    [ServiceFilter(typeof(AuthenticatedUserAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] IRegisterUseCase useCase,
        [FromBody] RequestRegisterDoctor request)
    {
        var result = await useCase.RegisterDoctorAsync(request);

        return ResponseCreate(result);
    }

    [HttpPut("change-password")]
    [ServiceFilter(typeof(AuthenticatedDoctorAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromServices] IChangePasswordUseCase useCase,
        [FromBody] RequestChangePassword request)
    {
        var result = await useCase.ChangePasswordAsync(request);

        return Response(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverAllAsync(
        [FromServices] IRecoverAllUseCase useCase,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await useCase.RecoverAllAsync(page, pageSize);

        return Response(result);
    }

    [HttpGet("cr/{cr}")]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecoverByCRAsync(
        [FromServices] IRecoverByCRUseCase useCase,
        [FromRoute] string cr)
    {
        var result = await useCase.RecoverByCRAsync(cr);

        return Response(result);
    }

    [HttpGet("id/{id}")]
    [ServiceFilter(typeof(AuthenticatedAttribute))]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseDoctor>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecoverByIdAsync(
        [FromServices] IRecoverByIdUseCase useCase,
        [FromRoute] Guid id)
    {
        var result = await useCase.RecoverByIdAsync(id);

        return Response(result);
    }
}
