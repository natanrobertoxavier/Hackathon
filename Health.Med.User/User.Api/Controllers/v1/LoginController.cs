using Microsoft.AspNetCore.Mvc;
using System.Net;
using User.Application.UseCase.Login;
using User.Communication.Request;
using User.Communication.Response;

namespace User.Api.Controllers.v1;

public class LoginController : HealthMedController
{

    [HttpPost]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverByCRPasswordAsync(
        [FromServices] ILoginUseCase useCase,
        [FromBody] RequestLoginUser request)
    {
        var result = await useCase.LoginAsync(request);

        return Response(
            result,
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Unauthorized);
    }
}
