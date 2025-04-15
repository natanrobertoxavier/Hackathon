using Doctor.Application.Mapping;
using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Doctor.Domain.Repositories.Contracts.ServiceDay;
using Serilog;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverScheduleByCRM;

public class RecoverScheduleByCRUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    ILogger logger) : IRecoverScheduleByCRUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseScheduleDoctor>> RecoverScheduleByCRAsync(string cr)
    {
        var output = new Result<ResponseScheduleDoctor>();

        try
        {
            _logger.Information($"Início {nameof(RecoverScheduleByCRAsync)}.");

            var entity = await _doctorReadOnlyrepository.RecoverByCRAsync(cr);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverScheduleByCRAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToResponseSchedule());
                _logger.Information($"Fim {nameof(RecoverScheduleByCRAsync)}.");
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
