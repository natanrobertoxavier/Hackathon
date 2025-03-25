using Doctor.Api.Filters;
using Doctor.Application.UseCase.Specialty.Register;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Api.Controllers.v1;

public class SpecialtyController : HealthMedController
{
    [HttpPost]
    [ServiceFilter(typeof(AuthenticatedUserAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] IRegisterUseCase useCase,
        [FromBody] RequestRegisterSpecialty request)
    {
        var result = await useCase.RegisterSpecialtyAsync(request);

        return ResponseCreate(result);
    }
}
