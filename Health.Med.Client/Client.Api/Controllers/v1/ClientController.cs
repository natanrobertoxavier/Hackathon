using Client.Api.Filters;
using Client.Application.UseCase.ChangePassword;
using Client.Application.UseCase.Recover.RecoverAll;
using Client.Application.UseCase.Recover.RecoverByCPF;
using Client.Application.UseCase.Recover.RecoverByEmail;
using Client.Application.UseCase.Register;
using Client.Communication.Request;
using Client.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Client.Api.Controllers.v1;

public class ClientController : HealthMedController
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUserAsync(
        [FromServices] IRegisterUseCase useCase,
        [FromBody] RequestRegisterClient request)
    {
        var result = await useCase.RegisterClientAsync(request);

        return ResponseCreate(result);
    }

    [HttpPut("change-password")]
    [ServiceFilter(typeof(AuthenticatedClientAttribute))]
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
    [ServiceFilter(typeof(AuthenticatedUserAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverAllAsync(
        [FromServices] IRecoverAllUseCase useCase,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await useCase.RecoverAllAsync(page, pageSize);

        return Response(result);
    }

    [HttpGet("{email}")]
    [ServiceFilter(typeof(AuthenticatedAttribute))]
    [ProducesResponseType(typeof(Result<ResponseClient>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseClient>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseClient>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverByEmailAsync(
        [FromServices] IRecoverByEmailUseCase useCase,
        [FromRoute] string email)
    {
        var result = await useCase.RecoverByEmailAsync(email);

        return Response(result);
    }

    [HttpGet("cpf/{cpf}")]
    [ServiceFilter(typeof(AuthenticatedUserAttribute))]
    [ProducesResponseType(typeof(Result<ResponseClient>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseClient>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseClient>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverByCPFAsync(
        [FromServices] IRecoverByCPFUseCase useCase,
        [FromRoute] string cpf)
    {
        var result = await useCase.RecoverByCPFAsync(cpf);

        return Response(result);
    }

    [HttpGet("basic-info/{email}")]
    [ServiceFilter(typeof(AuthenticatedAttribute))]
    [ProducesResponseType(typeof(Result<ResponseClientBasicInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseClientBasicInfo>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseClientBasicInfo>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverBasicInformationByEmailAsync(
        [FromServices] IRecoverByEmailUseCase useCase,
        [FromRoute] string email)
    {
        var result = await useCase.RecoverBasicInformationByEmailAsync(email);

        return Response(result);
    }
}
