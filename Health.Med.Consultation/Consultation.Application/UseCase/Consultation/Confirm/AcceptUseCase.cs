
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.UseCase.Consultation.Validate;
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
    IDoctorServiceApi doctorServiceApi,
    IWorkUnit workUnit,
    TokenController tokenController,
    ILogger logger) : IAcceptUseCase
{
    private readonly IValidateUseCase _validateUseCase = validateUseCase;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly IDoctorServiceApi _doctorServiceApi = doctorServiceApi;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly TokenController _tokenController = tokenController;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> AcceptConsultationAsync(Guid consultationId, string token)
    {
        using (LogContext.PushProperty("Operation", nameof(AcceptConsultationAsync)))
        {
            var output = new Result<MessageResult>();

            try
            {
                _logger.Information("Iniciando aceite de consulta.");

                var doctorId = await ValidateDoctorToken(token);
                await Validate(consultationId, doctorId);
                await _consultationWriteOnlyrepository.AcceptConsultationAsync(consultationId, DateTime.UtcNow);
                await _workUnit.CommitAsync();

                var successMesagem = "Consulta aceita com sucesso";
                output.Succeeded(new MessageResult(successMesagem));
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

    private async Task<Guid> ValidateDoctorToken(string token)
    {
        var email = _tokenController.RecoverEmail(token);
        var doctor = await _doctorServiceApi.RecoverByEmailAsync(email);
        if (!doctor.Success)
        {
            _logger.Error(doctor.Error);
            throw new ValidationErrorsException(new List<string>() { doctor.Error });
        }

        return doctor.Data.DoctorId;
    }

    private async Task Validate(Guid consultationId, Guid doctorId)
    {
        var thereIsConsultation = await _validateUseCase.ValidateConsultationIdAsync(consultationId, doctorId);

        if (!thereIsConsultation.IsSuccess() || !thereIsConsultation.Data)
            throw new ValidationErrorsException(new List<string> { "Consulta não encontrada." });
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