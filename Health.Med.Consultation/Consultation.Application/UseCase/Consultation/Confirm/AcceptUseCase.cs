using Consultation.Application.UseCase.Consultation.Validate;
using Consultation.Application.UseCase.SendEmailClient;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Consultation.Domain.Services;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using Serilog.Context;
using TokenService.Manager.Controller;

namespace Consultation.Application.UseCase.Consultation.Confirm;

public class AcceptUseCase(
    IValidateUseCase validateUseCase,
    IConsultationWriteOnly consultationWriteOnlyrepository,
    ISendEmailClientUseCase sendEmailClientUseCase,
    IDoctorServiceApi doctorServiceApi,
    IWorkUnit workUnit,
    TokenController tokenController,
    ILogger logger) : IAcceptUseCase
{
    private readonly IValidateUseCase _validateUseCase = validateUseCase;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly ISendEmailClientUseCase _sendEmailClientUseCase = sendEmailClientUseCase;
    private readonly IDoctorServiceApi _doctorServiceApi = doctorServiceApi;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly TokenController _tokenController = tokenController;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResultAcceptConsultation>> AcceptConsultationAsync(Guid consultationId, string token)
    {
        using (LogContext.PushProperty("Operation", nameof(AcceptConsultationAsync)))
        {
            var output = new Result<MessageResultAcceptConsultation>();

            try
            {
                _logger.Information("Iniciando aceite de consulta.");

                var doctor = await ValidateDoctorToken(token);
                var consultationDate = await Validate(consultationId, doctor.DoctorId);

                await _consultationWriteOnlyrepository.AcceptConsultationAsync(consultationId, DateTime.UtcNow);
                await _workUnit.CommitAsync();
                await SendEmailToClient(consultationId, consultationDate, doctor);

                var successMesagem = "Consulta aceita com sucesso";
                output.Succeeded(new MessageResultAcceptConsultation(successMesagem, CreateLinkRedirect(consultationDate)));
                _logger.Information(successMesagem);
            }
            catch (ValidationErrorsException ex)
            {
                LogValidationErrors(ex);
                output.Failure(ex.ErrorMessages);
            }
            catch (Exception ex)
            {
                LogUnexpectedError(ex);
                output.Failure(new List<string> { $"Algo deu errado: {ex.Message}" });
            }

            return output;
        }
    }

    private async Task<Domain.ModelServices.DoctorResult> ValidateDoctorToken(string token)
    {
        var email = _tokenController.RecoverEmail(token);
        var doctor = await _doctorServiceApi.RecoverByEmailAsync(email);
        if (!doctor.Success)
        {
            _logger.Error(doctor.Error);
            throw new ValidationErrorsException(new List<string>() { doctor.Error });
        }

        return doctor.Data;
    }

    private async Task<DateTime> Validate(Guid consultationId, Guid doctorId)
    {
        var dateConsultation = await _validateUseCase.ValidateConsultationIdAsync(consultationId, doctorId);

        if (!dateConsultation.IsSuccess() || dateConsultation.Data == default)
            throw new ValidationErrorsException(new List<string> { "Consulta não encontrada." });

        return dateConsultation.Data;
    }

    private static string CreateLinkRedirect(DateTime consultationDateTime)
    {
        var text = Uri.EscapeDataString("Consulta Médica");
        var details = Uri.EscapeDataString("Consulta confirmada");
        var location = Uri.EscapeDataString("Clínica Health Med");

        var dates = CreateDateTimeSchedule(consultationDateTime);

        return $"https://www.google.com/calendar/render?action=TEMPLATE&text={text}&details={details}&location={location}&dates={dates}";
    }

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

    private async Task SendEmailToClient(Guid consultationId, DateTime consultationDate, Domain.ModelServices.DoctorResult doctor)
    {
        _logger.Information($"Início do envio de e-mail para o cliente.");
        var request = new RequestRegisterConsultation(doctor.DoctorId, consultationDate);

        await _sendEmailClientUseCase
            .SendEmailConfirmationConsultationClientAsync(
                consultationId,
                new RequestRegisterConsultation(doctor.DoctorId, consultationDate), 
                doctor, 
                Domain.Entities.Enum.TemplateEmailEnum.ConfirmationConsultationClientEmail);

        _logger.Information($"Fim do envio de e-mail para o cliente.");
    }

    private void LogValidationErrors(ValidationErrorsException ex)
    {
        var errorMessage = $"Ocorreram erros de validação: {string.Join(", ", ex.ErrorMessages)}.";
        _logger.Error(ex, errorMessage);
    }

    private void LogUnexpectedError(Exception ex)
    {
        var errorMessage = $"Algo deu errado: {ex.Message}";
        _logger.Error(ex, errorMessage);
    }
}