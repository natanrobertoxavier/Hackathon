using Doctor.Application.Mapping;
using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Serilog;

namespace Doctor.Application.UseCase.Doctor.Recover.RecoverByEmail;

public class RecoverByEmailUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    ILogger logger) : IRecoverByEmailUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseDoctor>> RecoverByEmailAsync(string email)
    {
        var output = new Result<ResponseDoctor>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByEmailAsync)}.");

            var entity = await _doctorReadOnlyrepository.RecoverByEmailAsync(email);

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
