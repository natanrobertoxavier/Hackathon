using Doctor.Application.Mapping;
using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Serilog;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverById;

public class RecoverByIdUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    ILogger logger) : IRecoverByIdUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseDoctor>> RecoverByIdAsync(Guid doctorId)
    {
        var output = new Result<ResponseDoctor>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByIdAsync)}.");

            var entity = await _doctorReadOnlyrepository.RecoverByIdAsync(doctorId);

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
}
