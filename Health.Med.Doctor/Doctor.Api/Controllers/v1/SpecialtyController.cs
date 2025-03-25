using Doctor.Api.Filters;
using Doctor.Application.UseCase.Specialty.Recover.RecoverAll;
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

    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<ResponseSpecialty>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<IEnumerable<ResponseSpecialty>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<IEnumerable<ResponseSpecialty>>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecoverAllAsync(
        [FromServices] IRecoverAllUseCase useCase,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await useCase.RecoverAllAsync(page, pageSize);

        return Response(result);
    }
}
