using Consultation.Api.Filters;
using Consultation.Application.UseCase.Consultation.Confirm;
using Consultation.Application.UseCase.Consultation.Register;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Microsoft.AspNetCore.Mvc;

namespace Consultation.Api.Controllers.v1;

public class ConsultationController : HealthMedController
{
    [HttpPost]
    [ServiceFilter(typeof(AuthenticatedAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterConsultationAsync(
        [FromServices] IRegisterUseCase useCase,
        [FromBody] RequestRegisterConsultation request)
    {
        var result = await useCase.RegisterConsultationAsync(request);

        return ResponseCreate(result);
    }

    [HttpPost("confirm/{id}")]
    //[ServiceFilter(typeof(AuthenticatedAttribute))]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmConsultationAsync(
        [FromServices] IConfirmUseCase useCase,
        [FromRoute] Guid id)
    {
        var result = await useCase.ConfirmConsultationAsync(id);

        return ResponseCreate(result);
    }
}
