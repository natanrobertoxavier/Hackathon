using Client.Api.Filters;
using Client.Application.UseCase.ChangePassword;
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
}
