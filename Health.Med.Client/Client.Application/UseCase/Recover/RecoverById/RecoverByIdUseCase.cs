using Client.Application.Mapping;
using Client.Communication.Response;
using Client.Domain.Repositories.Contracts;
using Serilog;

namespace Client.Application.UseCase.Recover.RecoverById;

public class RecoverByIdUseCase(
    IClientReadOnly clientReadOnlyrepository,
    ILogger logger) : IRecoverByIdUseCase
{
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseClient>> RecoverByIdAsync(Guid clientId)
    {
        var output = new Result<ResponseClient>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByIdAsync)}.");

            var entity = await _clientReadOnlyrepository.RecoverByIdAsync(clientId);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverByIdAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToResponse());
                _logger.Information($"Fim {nameof(RecoverByIdAsync)}.");
            }
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);
            _logger.Error(ex, errorMessage);
            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }

    public async Task<Result<ResponseClientBasicInfo>> RecoverBasicInformationByIdAsync(Guid clientId)
    {
        var output = new Result<ResponseClientBasicInfo>();

        try
        {
            _logger.Information($"Início {nameof(RecoverBasicInformationByIdAsync)}.");

            var entity = await _clientReadOnlyrepository.RecoverByIdAsync(clientId);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverBasicInformationByIdAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToBasicResponse());
                _logger.Information($"Fim {nameof(RecoverBasicInformationByIdAsync)}.");
            }
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);
            _logger.Error(ex, errorMessage);
            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }
}
