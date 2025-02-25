using Microsoft.AspNetCore.Mvc;
using User.Api.Filters;
using User.Application.UseCase.Recover.RecoverByEmail;
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

    [HttpGet("{email}")]
    [ServiceFilter(typeof(AuthenticatedUserAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecoverByEmailAsync(
        [FromServices] IRecoverByEmailUseCase useCase,
        [FromRoute] string email)
    {
        var result = await useCase.RecoverByEmailAsync(email);

        return Response(result);
    }
}
