using Consultation.Application.UseCase.Consultation.Refuse;
using Consultation.Application.UseCase.Consultation.Validate;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories.Contracts;
using Consultation.Domain.Repositories;
using Consultation.Domain.Services;
using Health.Med.Exceptions.ExceptionBase;
using Serilog.Context;
using TokenService.Manager.Controller;
using Serilog;
using Consultation.Communication.Request;

namespace Consultation.Application.UseCase.Consultation.ClientCancel;

public class ClientCancelUseCase(
    IValidateUseCase validateUseCase,
    IConsultationWriteOnly consultationWriteOnlyrepository,
    IClientServiceApi clientServiceApi,
    IWorkUnit workUnit,
    TokenController tokenController,
    ILogger logger) : IClientCancelUseCase
{
    private readonly IValidateUseCase _validateUseCase = validateUseCase;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly IClientServiceApi _clientServiceApi = clientServiceApi;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly TokenController _tokenController = tokenController;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> ClientCancelConsultationAsync(RequestClientCancel request)
    {
        using (LogContext.PushProperty("Operation", nameof(ClientCancelConsultationAsync)))
        {
            var output = new Result<MessageResult>();

            try
            {
                _logger.Information("Iniciando cancelamento de consulta.");

                var clientId = await ValidateClientToken(request.Key);
                await Validate(request.Pin, clientId);
                await _consultationWriteOnlyrepository.ClientCancelConsultationAsync(request.Pin, request.Reason, DateTime.UtcNow);
                await _workUnit.CommitAsync();

                var successMesagem = "Consulta cancelada com sucesso";
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

    private async Task<Guid> ValidateClientToken(string token)
    {
        var email = _tokenController.RecoverEmail(token);
        var client = await _clientServiceApi.RecoverBasicInformationByEmailAsync(email, token);
        if (!client.Success)
        {
            _logger.Error(client.Error);
            throw new ValidationErrorsException(new List<string>() { client.Error });
        }

        return client.Data.Id;
    }

    private async Task Validate(Guid consultationId, Guid clientId)
    {
        var thereIsConsultation = await _validateUseCase.ValidateConsultationClientIdAsync(consultationId, clientId);

        if (!thereIsConsultation.IsSuccess() || thereIsConsultation.Data == default)
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