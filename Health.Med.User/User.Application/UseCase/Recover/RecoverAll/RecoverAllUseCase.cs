using Serilog;
using User.Application.Mapping;
using User.Communication.Response;
using User.Domain.Repositories.Contracts;

namespace User.Application.UseCase.Recover.RecoverAll;

public class RecoverAllUseCase(
    IUserReadOnly userReadOnlyrepository,
    ILogger logger) : IRecoverAllUseCase
{
    private readonly IUserReadOnly _userReadOnlyrepository = userReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<ResponseUser>>> RecoverAllAsync(int page, int pageSize)
    {
        var output = new Result<IEnumerable<ResponseUser>>();

        try
        {
            _logger.Information($"Início {nameof(RecoverAllAsync)}.");

            var entities = await _userReadOnlyrepository.RecoverAllAsync(Skip(page, pageSize), pageSize);

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
