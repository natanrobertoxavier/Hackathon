using Serilog;
using User.Application.Mapping;
using User.Communication.Response;
using User.Domain.Repositories.Contracts;

namespace User.Application.UseCase.Recover.RecoverByEmail;

public class RecoverByEmailUseCase(
    IUserReadOnly userReadOnlyrepository,
    ILogger logger) : IRecoverByEmailUseCase
{
    private readonly IUserReadOnly _userReadOnlyrepository = userReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseUser>> RecoverByEmailAsync(string email)
    {
        var output = new Result<ResponseUser>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByEmailAsync)}.");

            var entity = await _userReadOnlyrepository.RecoverByEmailAsync(email);

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
}
