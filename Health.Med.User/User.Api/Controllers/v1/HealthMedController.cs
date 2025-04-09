using Microsoft.AspNetCore.Mvc;
using System.Net;
using User.Communication.Response;

namespace User.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthMedController : ControllerBase
{
    protected new IActionResult ResponseCreate<TEntity>(
        Result<TEntity> output,
        HttpStatusCode successStatusCode = HttpStatusCode.Created,
        HttpStatusCode failStatusCode = HttpStatusCode.BadRequest) where TEntity : class
    {
        if (output.IsSuccess())
            return StatusCode((int)successStatusCode, output);

        return StatusCode((int)failStatusCode, output);
    }

    protected new IActionResult Response<TEntity>(
        Result<TEntity> output,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode notFoundStatusCode = HttpStatusCode.NotFound,
        HttpStatusCode failStatusCode = HttpStatusCode.UnprocessableEntity) where TEntity : class
    {
        if (output.IsSuccess())
        {
            var result = output.GetData();

            if (result is null || (result is IEnumerable<object> enumerable && !enumerable.Any()))
                return StatusCode((int)notFoundStatusCode, output);
            else
                return StatusCode((int)successStatusCode, output);
        }

        return StatusCode((int)failStatusCode, output);
    }
}
