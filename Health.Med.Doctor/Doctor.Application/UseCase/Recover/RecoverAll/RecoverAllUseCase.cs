using Doctor.Application.Mapping;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;

namespace Doctor.Application.UseCase.Recover.RecoverAll;

public class RecoverAllUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    ILogger logger) : IRecoverAllUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<ResponseDoctor>>> RecoverAllAsync(int page, int pageSize)
    {
        var output = new Result<IEnumerable<ResponseDoctor>>();

        try
        {
            _logger.Information($"Start {nameof(RecoverAllAsync)}.");

            var entities = await _doctorReadOnlyrepository.RecoverAllAsync(Skip(page, pageSize), pageSize);

            var response = entities.Select(entity => entity.ToResponse());

            _logger.Information($"End {nameof(RecoverAllAsync)}.");

            output.Succeeded(response);
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
