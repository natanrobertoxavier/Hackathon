using Consultation.Api.Filters;
using Consultation.Application.UseCase.Consultation.Confirm;
using Consultation.Application.UseCase.Consultation.Recover.RecoverByDoctorId;
using Consultation.Application.UseCase.Consultation.Refuse;
using Consultation.Application.UseCase.Consultation.Register;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

    [HttpGet("confirmed")]
    [ServiceFilter(typeof(AuthenticatedAttribute))]
    [ProducesResponseType(typeof(Result<ResponseConsultation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ResponseConsultation>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecoverAllConfirmedAsync(
        [FromServices] IRecoverByDoctorIdUseCase useCase)
    {
        var result = await useCase.RecoverByDoctorIdAsync();

        return Response(result);
    }

    [HttpGet("accept/{id}/{token}")]
    [ProducesResponseType(typeof(Result<MessageResultAcceptConsultation>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResultAcceptConsultation>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptConsultationAsync(
        [FromServices] IAcceptUseCase useCase,
        [FromRoute] Guid id,
        [FromRoute] string token)
    {
        var result = await useCase.AcceptConsultationAsync(id, token);

        if (result.IsSuccess())
            return Redirect(result.Data.UrlRedirect);

        return ResponseCreate(result);
    }

    [HttpGet("refuse/{id}/{token}")]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<MessageResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefuseConsultationAsync(
        [FromServices] IRefuseUseCase useCase,
        [FromRoute] Guid id,
        [FromRoute] string token)
    {
        var result = await useCase.RefuseConsultationAsync(id, token);

        return ResponseCreate(result, successStatusCode: HttpStatusCode.OK);
    }
}
