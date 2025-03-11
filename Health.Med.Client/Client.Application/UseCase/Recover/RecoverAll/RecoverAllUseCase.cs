using Client.Application.Mapping;
using Client.Communication.Response;
using Client.Domain.Repositories.Contracts;
using Serilog;

namespace Client.Application.UseCase.Recover.RecoverAll;

public class RecoverAllUseCase(
    IClientReadOnly clientReadOnlyrepository,
    ILogger logger) : IRecoverAllUseCase
{
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<ResponseClient>>> RecoverAllAsync(int page, int pageSize)
    {
        var output = new Result<IEnumerable<ResponseClient>>();

        try
        {
            _logger.Information($"Início {nameof(RecoverAllAsync)}.");

            var entities = await _clientReadOnlyrepository.RecoverAllAsync(Skip(page, pageSize), pageSize);

            var response = entities.Select(entity => entity.ToResponse());

            output.Succeeded(response);

            _logger.Information($"Fim {nameof(RecoverAllAsync)}.");
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

            _logger.Error(ex, errorMessage);

            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }

    private static int Skip(int page, int pageSize) =>
        (page - 1) * pageSize;
}
