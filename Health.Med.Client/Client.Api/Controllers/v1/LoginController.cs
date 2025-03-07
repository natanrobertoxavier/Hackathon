using Client.Communication.Request;
using Client.Communication.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Client.Api.Controllers.v1;
public class LoginController : HealthMedController
{

    [HttpPost]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverByCRPasswordAsync(
        [FromServices] ILoginUseCase useCase,
        [FromBody] RequestLoginClient request)
    {
        var result = await useCase.LoginAsync(request);

        return Response(
            result,
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Unauthorized);
    }
}
