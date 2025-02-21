using Doctor.Application.UseCase.Recover.RecoverAll;
using Doctor.Application.UseCase.Register;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Api.Controllers.v1;

public class DoctorController : HealthMedController
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterUserAsync(
        [FromServices] IRegisterDoctorUseCase useCase,
        [FromBody] RequestRegisterDoctor request)
    {
        var result = await useCase.RegisterDoctorAsync(request);

        return ResponseCreate(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoverAllAsync(
        [FromServices] IRecoverDoctorUseCase useCase,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await useCase.RecoverAllAsync(page, pageSize);

        return Response(result);
    }
}
