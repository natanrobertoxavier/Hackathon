using Doctor.Api.Filters;
using Doctor.Application.UseCase.ServiceDay.Delete;
using Doctor.Application.UseCase.ServiceDay.Register;
using Doctor.Application.UseCase.ServiceDay.Update;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class ServiceDayController : HealthMedController
{
    [HttpPost]
    [ServiceFilter(typeof(AuthenticatedDoctorAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] IRegisterUseCase useCase,
        [FromBody] RequestServiceDay request)
    {
        var result = await useCase.RegisterServiceDayAsync(request);

        return ResponseCreate(result);
    }

    [HttpPut]
    [ServiceFilter(typeof(AuthenticatedDoctorAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] IUpdateUseCase useCase,
        [FromBody] RequestServiceDay request)
    {
        var result = await useCase.UpdateServiceDayAsync(request);

        return ResponseCreate(result);
    }

    [HttpDelete]
    [ServiceFilter(typeof(AuthenticatedDoctorAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] IDeleteUseCase useCase,
        [FromBody] RequestDeleteServiceDay request)
    {
        var result = await useCase.DeleteServiceDayAsync(request);

        return ResponseCreate(result);
    }
}