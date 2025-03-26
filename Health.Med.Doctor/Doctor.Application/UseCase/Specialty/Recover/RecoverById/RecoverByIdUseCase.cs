using Doctor.Application.Mapping;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.Specialty;
using Serilog;

namespace Doctor.Application.UseCase.Specialty.Recover.RecoverById;

public class RecoverByIdUseCase(
    ISpecialtyReadOnly specialtyReadOnlyrepository,
    ILogger logger) : IRecoverByIdUseCase
{
    private readonly ISpecialtyReadOnly _specialtyReadOnlyrepository = specialtyReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseSpecialty>> RecoverByIdAsync(Guid id)
    {
        var output = new Result<ResponseSpecialty>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByIdAsync)}.");

            var entity = await _specialtyReadOnlyrepository.RecoverByIdAsync(id);

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

            _logger.Information($"Fim {nameof(RecoverByIdAsync)}.");
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
