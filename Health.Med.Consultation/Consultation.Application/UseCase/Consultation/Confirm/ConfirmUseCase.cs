
using Consultation.Application.Services.LoggedClientService;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using Serilog.Context;

namespace Consultation.Application.UseCase.Consultation.Confirm;

public class ConfirmUseCase(
    ILoggedClient loggedClient,
    IConsultationWriteOnly consultationWriteOnlyrepository,
    IWorkUnit workUnit,
    ILogger logger) : IConfirmUseCase
{
    private readonly ILoggedClient _loggedClient = loggedClient;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> ConfirmConsultationAsync(Guid consultationId)
    {
        using (LogContext.PushProperty("Operation", nameof(ConfirmConsultationAsync)))
        {
            var output = new Result<MessageResult>();

            try
            {
                _logger.Information("Iniciando cadastro de consulta.");
                
                await _consultationWriteOnlyrepository.ConfirmConsultationAsync(consultationId, DateTime.UtcNow);
                await _workUnit.CommitAsync();

                var successMesagem = "Consulta confirmada com suceso";
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