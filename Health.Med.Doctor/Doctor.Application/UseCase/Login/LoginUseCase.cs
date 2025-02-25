using Doctor.Application.Mapping;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Application.UseCase.Login;

public class LoginUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    PasswordEncryptor passwordEncryptor,
    TokenController tokenController,
    ILogger logger) : ILoginUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly TokenController _tokenController = tokenController;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseLogin>> LoginAsync(RequestLoginDoctor request)
    {
        var output = new Result<ResponseLogin>();

        try
        {
            _logger.Information($"Início {nameof(LoginAsync)}.");

            var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

            var entity = await _doctorReadOnlyrepository.RecoverByCRPasswordAsync(request.CR.ToUpper(), encryptedPassword);

            if (entity?.Id == Guid.Empty)
            {
                _logger.Information($"Fim {nameof(LoginAsync)}. Não foram encontrados dados.");

                throw new InvalidLoginException();
            }
            else
            {
                var token = _tokenController.GenerateToken(entity.Email);

                output.Succeeded(entity.ToResponseLogin(token));

                _logger.Information($"Fim {nameof(LoginAsync)}.");
            }
        }
        catch (InvalidLoginException ex)
        {
            output.Failure(new List<string>() { ex.Message });
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

            output.Failure(new List<string>() { errorMessage });

            _logger.Error(ex, errorMessage);
        }

        return output;
    }
}
