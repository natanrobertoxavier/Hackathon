using Consultation.Application.Extensions;
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.Settings;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Entities.Enum;
using Consultation.Domain.Messages.DomainEvents;
using Consultation.Domain.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;

namespace Consultation.Application.UseCase.SendEmailClient;

public class SendEmailClientUseCase(
    IMediator mediator,
    IOptions<TemplateSettings> options,
    IDoctorServiceApi doctorServiceApi,
    ILoggedClient loggedClient,
    ILogger logger) : ISendEmailClientUseCase
{
    private readonly IMediator _mediator = mediator;
    private readonly TemplateSettings _options = options.Value;
    private readonly IDoctorServiceApi _doctorServiceApi = doctorServiceApi;
    private readonly ILoggedClient _loggedClient = loggedClient;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> SendEmailClientAsync(RequestRegisterConsultation request, TemplateEmailEnum template)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(SendEmailClientAsync)}.");

            var client = _loggedClient.GetLoggedClient();
            var doctor = await _doctorServiceApi.RecoverByIdAsync(request.DoctorId);
            var consultationDateTime = request.ConsultationDate.ToSPDateZone();

            var content = GetEmailBody(template)
                .Replace("@@@CLIENT@@@", client.PreferredName)
                .Replace("@@@ESPECIALTY@@@", doctor.Data.SpecialtyDoctor.Description)
                .Replace("@@@DOCTORNAME@@@", doctor.Data.Name)
                .Replace("@@@DATE@@@", consultationDateTime.ToString("dd/MM/yyyy"))
                .Replace("@@@HOUR@@@", consultationDateTime.ToString("HH:mm"));

            await _mediator.Publish(new SendEmailClientEvent(
                [client.Email],
                $"{client.PreferredName}, {_options.Subject}",
                content)
            );

            output.Succeeded(new MessageResult("Email enviado com sucesso"));

            _logger.Information($"Fim {nameof(SendEmailClientAsync)}.");
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

            _logger.Error(ex, errorMessage);

            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }

    private string GetEmailBody(TemplateEmailEnum template)
    {
        var path = $"{_options.PathTemplateClient}.{template.GetDescription()}.html";

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
        {
            if (stream is null)
                throw new KeyNotFoundException($"Template {template.GetDescription()} não encontrado.");

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
