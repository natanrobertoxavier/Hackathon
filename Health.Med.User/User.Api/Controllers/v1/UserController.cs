using Microsoft.AspNetCore.Mvc;
using User.Application.UseCase.Register;
using User.Communication.Request;
using User.Communication.Response;

namespace User.Api.Controllers.v1;

public class UserController : HealthMedController
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUserAsync(
        [FromServices] IRegisterUseCase useCase,
        [FromBody] RequestRegisterUser request)
    {
        var result = await useCase.RegisterUserAsync(request);

        return ResponseCreate(result);
    }
}
