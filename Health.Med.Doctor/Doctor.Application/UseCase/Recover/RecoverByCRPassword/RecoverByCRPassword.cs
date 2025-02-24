using Doctor.Application.Mapping;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Application.UseCase.Recover.RecoverByCRPassword;

public class RecoverByCRPassword(
    IDoctorReadOnly doctorReadOnlyrepository,
    PasswordEncryptor passwordEncryptor,
    ILogger logger) : IRecoverByCRPassword
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseLogin>> RecoverByCRPasswordAsync(RequestLoginDoctor request)
    {
        var output = new Result<ResponseLogin>();

        try
        {
            _logger.Information($"Início {nameof(RecoverByCRPasswordAsync)}.");

            var entity = await _doctorReadOnlyrepository.RecoverByCRPasswordAsync(request.CR.ToUpper(), request.Password);

            if (entity?.Id == Guid.Empty)
            {
                output.Succeeded(null);
                _logger.Information($"Fim {nameof(RecoverByCRPasswordAsync)}. Não foram encontrados dados.");
            }
            else
            {
                output.Succeeded(entity.ToResponseLogin());
                _logger.Information($"Fim {nameof(RecoverByCRPasswordAsync)}.");
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
