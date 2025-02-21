using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Doctor.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthMedController : ControllerBase
{
    protected new IActionResult ResponseCreate<TEntity>(
        Result<TEntity> result,
        HttpStatusCode successStatusCode = HttpStatusCode.Created,
        HttpStatusCode failStatusCode = HttpStatusCode.BadRequest) where TEntity : class
    {
        if (result.IsSuccess())
            return StatusCode((int) successStatusCode, result);

        return StatusCode((int) failStatusCode, result);
    }
}
