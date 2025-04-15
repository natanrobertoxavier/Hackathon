using Doctor.Application.Mapping;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Serilog;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverBySpecialtyId;

public class RecoverBySpecialtyIdUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    ILogger logger) : IRecoverBySpecialtyIdUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<ResponseDoctor>>> RecoverBySpecialtyIdAsync(Guid specialtyId)
    {
        var output = new Result<IEnumerable<ResponseDoctor>>();

        try
        {
            _logger.Information($"Início {nameof(RecoverBySpecialtyIdAsync)}.");

            var entities = await _doctorReadOnlyrepository.RecoverBySpecialtyIdAsync(specialtyId);

            if (entities is null || !entities.Any())
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverBySpecialtyIdAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entities.Select(entity => entity.ToResponse()));
                _logger.Information($"Fim {nameof(RecoverBySpecialtyIdAsync)}.");
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
