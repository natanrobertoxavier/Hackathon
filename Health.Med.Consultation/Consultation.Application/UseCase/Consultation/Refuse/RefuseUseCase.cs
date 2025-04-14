using Consultation.Application.UseCase.Consultation.Validate;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using Serilog.Context;

namespace Consultation.Application.UseCase.Consultation.Refuse;

public class RefuseUseCase(
    IValidateUseCase validateUseCase,
    IConsultationWriteOnly consultationWriteOnlyrepository,
    IWorkUnit workUnit,
    ILogger logger) : IRefuseUseCase
{
    private readonly IValidateUseCase _validateUseCase = validateUseCase;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RefuseConsultationAsync(Guid consultationId)
    {
        using (LogContext.PushProperty("Operation", nameof(RefuseConsultationAsync)))
        {
            var output = new Result<MessageResult>();

            try
            {
                _logger.Information("Iniciando aceite de consulta.");

                await Validate(consultationId);
                await _consultationWriteOnlyrepository.RefuseConsultationAsync(consultationId, DateTime.UtcNow);
                await _workUnit.CommitAsync();

                var successMesagem = "Consulta recusada com suceso";
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

    private async Task Validate(Guid consultationId)
    {
        var thereIsConsultation = await _validateUseCase.ValidateConsultationIdAsync(consultationId);

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