using Doctor.Application.Mapping;
using Doctor.Application.UseCase.Recover.RecoverAll;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;

namespace Doctor.Application.UseCase.Recover.RecoverByCR;

public class RecoverByCRUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    ILogger logger) : IRecoverByCRUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseDoctor>> RecoverByCRAsync(string cr)
    {
        var output = new Result<ResponseDoctor>();

        try
        {
            _logger.Information($"Start {nameof(RecoverByCRAsync)}.");

            var entity = await _doctorReadOnlyrepository.RecoverByCRAsync(cr);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"End {nameof(RecoverByCRAsync)}. Data not found");
            }
            else
            {
                output.Succeeded(entity.ToResponse());
                _logger.Information($"End {nameof(RecoverByCRAsync)}.");
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
