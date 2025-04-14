using Consultation.Application.Services.LoggedDoctor;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using Serilog.Context;

namespace Consultation.Application.UseCase.Consultation.Validate;

public class ValidateUseCase(
    IConsultationReadOnly consultationReadOnly,
    ILogger logger) : IValidateUseCase
{
    private readonly IConsultationReadOnly _consultationReadOnly = consultationReadOnly;
    private readonly ILogger _logger = logger;

    public async Task<Result<DateTime>> ValidateConsultationIdAsync(Guid consultationId, Guid doctorId)
    {
        using (LogContext.PushProperty("Operation", nameof(ValidateConsultationIdAsync)))
        {
            var output = new Result<DateTime>();

            try
            {
                _logger.Information("Iniciando aceite de consulta.");

                var response = await _consultationReadOnly.ThereIsConsultationAsync(consultationId, doctorId);

                output.Succeeded(response);
                _logger.Information($"A consulta pertence ao médico logado? {response == default}");
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
