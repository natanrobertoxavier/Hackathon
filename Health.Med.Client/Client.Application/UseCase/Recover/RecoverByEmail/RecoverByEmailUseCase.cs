using Client.Application.Mapping;
using Client.Communication.Response;
using Client.Domain.Repositories.Contracts;
using Serilog;

namespace Client.Application.UseCase.Recover.RecoverByEmail;

public class RecoverByEmailUseCase(
    IClientReadOnly clientReadOnlyrepository,
    ILogger logger) : IRecoverByEmailUseCase
{
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseClient>> RecoverByEmailAsync(string email)
    {
        var output = new Result<ResponseClient>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByEmailAsync)}.");

            var entity = await _clientReadOnlyrepository.RecoverByEmailAsync(email);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverByEmailAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToResponse());
                _logger.Information($"Fim {nameof(RecoverByEmailAsync)}.");
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

    public async Task<Result<ResponseClientBasicInfo>> RecoverBasicInformationByEmailAsync(string email)
    {
        var output = new Result<ResponseClientBasicInfo>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByEmailAsync)}.");

            var entity = await _clientReadOnlyrepository.RecoverByEmailAsync(email);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverByEmailAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToBasicResponse());
                _logger.Information($"Fim {nameof(RecoverByEmailAsync)}.");
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
