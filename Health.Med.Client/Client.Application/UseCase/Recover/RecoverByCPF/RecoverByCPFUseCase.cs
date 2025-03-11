using Client.Application.Mapping;
using Client.Communication.Response;
using Client.Domain.Repositories.Contracts;
using Serilog;

namespace Client.Application.UseCase.Recover.RecoverByCPF;

public class RecoverByCPFUseCase(
    IClientReadOnly clientReadOnlyrepository,
    ILogger logger) : IRecoverByCPFUseCase
{
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseClient>> RecoverByCPFAsync(string cpf)
    {
        var output = new Result<ResponseClient>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByCPFAsync)}.");

            var entity = await _clientReadOnlyrepository.RecoverByCPFAsync(cpf);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverByCPFAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToResponse());
                _logger.Information($"Fim {nameof(RecoverByCPFAsync)}.");
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
