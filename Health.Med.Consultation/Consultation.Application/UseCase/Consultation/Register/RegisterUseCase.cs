using Consultation.Application.Extensions;
using Consultation.Application.Mapping;
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.UseCase.SendEmailClient;
using Consultation.Application.UseCase.SendEmailDoctor;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.ModelServices;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Consultation.Domain.Services;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using Serilog.Context;
using System.Globalization;

namespace Consultation.Application.UseCase.Consultation.Register;

public class RegisterUseCase(
    ILoggedClient loggedClient,
    IConsultationReadOnly consultationReadOnlyrepository,
    IConsultationWriteOnly consultationWriteOnlyrepository,
    IDoctorServiceApi doctorServiceApi,
    IWorkUnit workUnit,
    ISendEmailClientUseCase sendEmailClientUseCase,
    ISendEmailDoctorUseCase sendEmailDoctorUseCase,
    ILogger logger) : IRegisterUseCase
{
    private readonly ILoggedClient _loggedClient = loggedClient;
    private readonly IConsultationReadOnly _consultationReadOnlyrepository = consultationReadOnlyrepository;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly IDoctorServiceApi _doctorServiceApi = doctorServiceApi;
    private readonly ISendEmailClientUseCase _sendEmailClientUseCase = sendEmailClientUseCase;
    private readonly ISendEmailDoctorUseCase _sendEmailDoctorUseCase = sendEmailDoctorUseCase;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILogger _logger = logger;

    public async Task<Communication.Response.Result<MessageResult>> RegisterConsultationAsync(RequestRegisterConsultation request)
    {
        using (LogContext.PushProperty("Operation", nameof(RegisterConsultationAsync)))
        using (LogContext.PushProperty("ClientId", _loggedClient.GetLoggedClientId()))
        {
            var output = new Communication.Response.Result<MessageResult>();

            try
            {
                _logger.Information("Iniciando cadastro de consulta.");

                var clientId = _loggedClient.GetLoggedClientId();
                var doctor = await ValidateAndGetDoctorAsync(request, clientId);
                var entity = request.ToEntity(clientId);
                await _consultationWriteOnlyrepository.AddAsync(entity);
                await _workUnit.CommitAsync();

                var consultationId = entity.Id;

                await SendEmailsAsync(request, doctor, consultationId);

                output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));
                _logger.Information("Cadastro de consulta finalizado com sucesso.");
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

    private async Task<DoctorResult> ValidateAndGetDoctorAsync(RequestRegisterConsultation request, Guid clientId)
    {
        _logger.Information($"Início da validação.");

        var validationResult = new RegisterValidator().Validate(request);

        var doctor = await _doctorServiceApi.RecoverByIdAsync(request.DoctorId);
        ValidateDoctor(doctor, validationResult, request);

        ValidateConsultationTime(request, validationResult);
        await ValidateClientAndDoctorScheduleAsync(request, clientId, validationResult);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }

        return doctor.Data;
    }

    private static void ValidateDoctor(Domain.ModelServices.Result<DoctorResult> doctor, FluentValidation.Results.ValidationResult validationResult, RequestRegisterConsultation request)
    {
        if (!doctor.Success)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("doctorApi", doctor.Error));
        else if (doctor.Data is null)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("doctor", ErrorsMessages.DoctorNotFound));
        else
        {
            var dayAvailable = ValidateServiceDayAvailable(request.ConsultationDate, doctor.Data.ServiceDays);

            if (!dayAvailable)
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("doctor", ErrorsMessages.DoctorNotAvailable, request.ConsultationDate));
        }
    }

    private static void ValidateConsultationTime(RequestRegisterConsultation request, FluentValidation.Results.ValidationResult validationResult)
    {
        if (!HourIsValid(request.ConsultationDate))
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("consultationHour", ErrorsMessages.ConsultationHourInvalid, request.ConsultationDate));

        var consultationDateUtc = request.ConsultationDate.ToUniversalTime();
        if (consultationDateUtc <= DateTime.UtcNow)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("consultationDate", ErrorsMessages.ConsultationDateInPast, request.ConsultationDate));
    }

    private async Task ValidateClientAndDoctorScheduleAsync(RequestRegisterConsultation request, Guid clientId, FluentValidation.Results.ValidationResult validationResult)
    {
        var trimmedDate = request.ConsultationDate.TrimMilliseconds();

        if (await _consultationReadOnlyrepository.ThereIsConsultationForClient(clientId, trimmedDate))
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("client", string.Format(ErrorsMessages.ClientAlreadyConsultationSchedule, request.ConsultationDate)));

        if (await _consultationReadOnlyrepository.ThereIsConsultationForDoctor(request.DoctorId, trimmedDate))
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("doctor", string.Format(ErrorsMessages.DoctorAlreadyConsultationSchedule, request.ConsultationDate)));
    }

    private static bool HourIsValid(DateTime consultationDate) =>
        consultationDate.Minute == 0 || consultationDate.Minute == 30;

    private static bool ValidateServiceDayAvailable(DateTime consultationDate, IEnumerable<ResponseServiceDay> serviceDays)
    {
        var consultationTime = consultationDate.TimeOfDay;
        var consultationDay = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(consultationDate.DayOfWeek);

        return serviceDays.Any(day =>
            day.Day.Equals(consultationDay, StringComparison.OrdinalIgnoreCase) &&
            consultationTime >= day.StartTime && consultationTime <= day.EndTime);

    }

    private async Task SendEmailsAsync(RequestRegisterConsultation request, DoctorResult doctor, Guid consultationId)
    {
        await SendEmailToClient(request, doctor, consultationId);
        await SendEmailToDoctor(request, doctor, consultationId);
    }

    private async Task SendEmailToClient(RequestRegisterConsultation request, DoctorResult doctor, Guid consultationId)
    {
        _logger.Information($"Início do envio de e-mail para o cliente.");

        await _sendEmailClientUseCase.SendEmailSchedulingConsultationClientAsync(request, doctor, consultationId, Domain.Entities.Enum.TemplateEmailEnum.ConsultationSchedulingClientEmail);
        
        _logger.Information($"Fim do envio de e-mail para o cliente.");
    }

    private async Task SendEmailToDoctor(RequestRegisterConsultation request, DoctorResult doctor, Guid consultationId)
    {
        _logger.Information($"Início do envio de e-mail para o médico.");

        await _sendEmailDoctorUseCase.SendEmailDoctorAsync(request, doctor, consultationId, Domain.Entities.Enum.TemplateEmailEnum.ConsultationSchedulingDoctorEmail);

        _logger.Information($"Fim do envio de e-mail para o médico.");
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