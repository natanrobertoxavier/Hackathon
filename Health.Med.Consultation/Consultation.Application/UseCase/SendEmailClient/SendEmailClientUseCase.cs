using Consultation.Application.Extensions;
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.Settings;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Entities.Enum;
using Consultation.Domain.Messages.DomainEvents;
using Consultation.Domain.ModelServices;
using Consultation.Domain.Repositories;
using Consultation.Domain.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;
using TokenService.Manager.Controller;

namespace Consultation.Application.UseCase.SendEmailClient;

public class SendEmailClientUseCase(
    IMediator mediator,
    IOptions<TemplateSettings> options,
    ILoggedClient loggedClient,
    IClientServiceApi clientServiceApi,
    IConsultationReadOnly consultationReadOnlyrepository,
    TokenController tokenController,
    ILogger logger) : ISendEmailClientUseCase
{
    private readonly IMediator _mediator = mediator;
    private readonly TemplateSettings _options = options.Value;
    private readonly ILoggedClient _loggedClient = loggedClient;
    private readonly IClientServiceApi _clientServiceApi = clientServiceApi;
    private readonly IConsultationReadOnly _consultationReadOnlyrepository = consultationReadOnlyrepository;
    private readonly TokenController _tokenController = tokenController;
    private readonly ILogger _logger = logger;

    public async Task<Communication.Response.Result<MessageResult>> SendEmailSchedulingConsultationClientAsync(RequestRegisterConsultation request, DoctorResult doctor, Guid consultationId, TemplateEmailEnum template)
    {
        var output = new Communication.Response.Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(SendEmailSchedulingConsultationClientAsync)}.");

            var client = _loggedClient.GetLoggedClient();
            var consultationDateTime = request.ConsultationDate;
            var token = _tokenController.GenerateToken(client.Email);

            var content = GetEmailBody(template)
                .Replace("@@@CLIENT@@@", client.PreferredName.Trim())
                .Replace("@@@ESPECIALTY@@@", doctor.SpecialtyDoctor.Description.Trim())
                .Replace("@@@DOCTORNAME@@@", doctor.Name.Trim())
                .Replace("@@@DATE@@@", consultationDateTime.ToString("dd/MM/yyyy"))
                .Replace("@@@HOUR@@@", consultationDateTime.ToString("HH:mm"))
                .Replace("@@@REFUSELINK@@@", CreateRefuseLink(consultationId, token));

            await _mediator.Publish(new SendEmailClientEvent(
                [client.Email],
                $"{client.PreferredName}, {_options.ClientSettings.Subject}",
                content)
            );

            output.Succeeded(new MessageResult("Email enviado com sucesso"));

            _logger.Information($"Fim {nameof(SendEmailSchedulingConsultationClientAsync)}.");
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

            _logger.Error(ex, errorMessage);

            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }

    public async Task<Communication.Response.Result<MessageResult>> SendEmailConfirmationConsultationClientAsync(Guid consultationId, RequestRegisterConsultation request, DoctorResult doctor, TemplateEmailEnum template)
    {
        var output = new Communication.Response.Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(SendEmailConfirmationConsultationClientAsync)}.");

            var client = await GetClientAsync(consultationId);
            var consultationDateTime = request.ConsultationDate;
            var token = _tokenController.GenerateToken(client.Email);

            var content = GetEmailBody(template)
                .Replace("@@@CLIENT@@@", client.PreferredName.Trim())
                .Replace("@@@ESPECIALTY@@@", doctor.SpecialtyDoctor.Description.Trim())
                .Replace("@@@DOCTORNAME@@@", doctor.Name.Trim())
                .Replace("@@@DATE@@@", consultationDateTime.ToString("dd/MM/yyyy"))
                .Replace("@@@HOUR@@@", consultationDateTime.ToString("HH:mm"))
                .Replace("@@@CALENDARDATE@@@", CreateDateTimeSchedule(consultationDateTime))
                .Replace("@@@REFUSELINK@@@", CreateRefuseLink(consultationId, token));

            await _mediator.Publish(new SendEmailClientEvent(
                [client.Email],
                $"{client.PreferredName}, {_options.ClientSettings.SubjectConfirmation}",
                content)
            );

            output.Succeeded(new MessageResult("Email enviado com sucesso"));

            _logger.Information($"Fim {nameof(SendEmailConfirmationConsultationClientAsync)}.");
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

            _logger.Error(ex, errorMessage);

            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }

    private async Task<ClientBasicInformationResult> GetClientAsync(Guid consultationId)
    {
        var consultation = await _consultationReadOnlyrepository.GetConsultationByIdAsync(consultationId);
        var client = await _clientServiceApi.RecoverBasicInformationByClientIdAsync(consultation.ClientId);

        if (!client.Success)
            throw new KeyNotFoundException($"Cliente não encontrado. Erro: {client.Error}");

        return client.Data;
    }

    private string GetEmailBody(TemplateEmailEnum template)
    {
        var path = $"{_options.ClientSettings.PathTemplateClient}.{template.GetDescription()}.html";

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
        {
            if (stream is null)
                throw new KeyNotFoundException($"Template {template.GetDescription()} não encontrado.");

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }

    private string CreateRefuseLink(Guid consultationId, string token) =>
        $"{_options.ClientSettings.RefuseLink}?pin={consultationId}&key{token}";

    private static string CreateDateTimeSchedule(DateTime consultationDateTime)
    {
        var endAppointment = consultationDateTime.AddMinutes(30);

        return string.Concat(
            consultationDateTime.ToString("yyyy"),
            consultationDateTime.ToString("MM"),
            consultationDateTime.ToString("dd"),
            "T",
            consultationDateTime.ToString("HH"),
            consultationDateTime.ToString("mm"),
            consultationDateTime.ToString("ss"),
            "/",
            consultationDateTime.ToString("yyyy"),
            consultationDateTime.ToString("MM"),
            consultationDateTime.ToString("dd"),
            "T",
            endAppointment.ToString("HH"),
            endAppointment.ToString("mm"),
            endAppointment.ToString("ss"),
            "&ctz=America/Sao_Paulo");
    }
}
