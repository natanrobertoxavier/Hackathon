using Doctor.Persistence.Application.UseCase.Register;
using Doctor.Persistence.Communication.Request;
using Doctor.Persistence.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Persistence.Api.Controllers.v1;

public class DoctorController : HealthMedController
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterUserAsync(
        [FromServices] IRegisterDoctorUseCase useCase,
        [FromBody] RequestRegisterDoctor request)
    {
        var result = await useCase.RegisterDoctorAsync(request);

        return Ok(result);
    }
}
