using Microsoft.AspNetCore.Mvc;
using Notification.Application.UseCase.SendMail;
using Notification.Communication.Request;
using Notification.Communication.Response;

namespace Notification.Api.Controllers.v1;

public class EmailController : HealthMedController
{
    [HttpPost]
    [Route("Send")]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMail(
        [FromServices] ISendMailUseCase useCase,
        [FromBody] RequestSendMail request)
    {
        var response = await useCase.SendAsync(request);

        return Response(response);
    }
}
