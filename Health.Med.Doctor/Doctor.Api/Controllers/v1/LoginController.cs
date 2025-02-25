using Doctor.Application.UseCase.Login;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Doctor.Api.Controllers.v1;

public class LoginController : HealthMedController
{

    [HttpPost]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<ResponseLogin>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverByCRPasswordAsync(
        [FromServices] ILoginUseCase useCase,
        [FromBody] RequestLoginDoctor request)
    {
        var result = await useCase.LoginAsync(request);

        return Response(
            result,
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Unauthorized);
    }
}
