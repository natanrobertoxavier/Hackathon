using Doctor.Communication.Response;
using Doctor.Application.Mapping;
using Serilog;
using Doctor.Domain.Repositories.Contracts.Specialty;

namespace Doctor.Application.UseCase.Specialty.Recover.RecoverAll;

public class RecoverAllUseCase(
    ISpecialtyReadOnly specialtyReadOnlyrepository,
    ILogger logger) : IRecoverAllUseCase
{
    private readonly ISpecialtyReadOnly _specialtyReadOnlyrepository = specialtyReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<ResponseSpecialty>>> RecoverAllAsync(int page, int pageSize)
    {
        var output = new Result<IEnumerable<ResponseSpecialty>>();

        try
        {
            _logger.Information($"Início {nameof(RecoverAllAsync)}.");

            var entities = await _specialtyReadOnlyrepository.RecoverAllAsync(Skip(page, pageSize), pageSize);

            var response = entities.Select(entity => entity.ToResponse());

            if (!entities.Any())
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverAllAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entities.Select(entity => entity.ToResponse()));
                _logger.Information($"Fim {nameof(RecoverAllAsync)}.");
            }

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
