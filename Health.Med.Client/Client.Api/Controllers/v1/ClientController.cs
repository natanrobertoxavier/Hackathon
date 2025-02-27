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
}
