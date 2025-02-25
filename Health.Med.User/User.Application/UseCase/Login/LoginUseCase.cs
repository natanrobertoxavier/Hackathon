using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;
using User.Application.Mapping;
using User.Communication.Request;
using User.Communication.Response;
using User.Domain.Repositories.Contracts;

namespace User.Application.UseCase.Login;

public class LoginUseCase(
    IUserReadOnly userReadOnlyrepository,
    PasswordEncryptor passwordEncryptor,
    TokenController tokenController,
    ILogger logger) : ILoginUseCase
{
    private readonly IUserReadOnly _userReadOnlyrepository = userReadOnlyrepository;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly TokenController _tokenController = tokenController;
    private readonly ILogger _logger = logger;

    public async Task<Result<ResponseLogin>> LoginAsync(RequestLoginUser request)
    {
        var output = new Result<ResponseLogin>();

        try
        {
            _logger.Information($"Início {nameof(LoginAsync)}.");

            var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

            var entity = await _userReadOnlyrepository.RecoverByEmailPasswordAsync(request.Email.ToLower(), encryptedPassword);

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
