using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;
using User.Application.Services;
using User.Communication.Request;
using User.Communication.Response;
using User.Domain.Repositories;
using User.Domain.Repositories.Contracts;

namespace User.Application.UseCase.ChangePassword;

public class ChangePasswordUseCase(
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    ILogger logger,
    ILoggedUser loggedUser,
    IUserReadOnly userReadOnlyrepository,
    IUserWriteOnly userWriteOnlyrepository) : IChangePasswordUseCase
{
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;
    private readonly ILoggedUser _loggedUser = loggedUser;
    private readonly IUserReadOnly _userReadOnlyrepository = userReadOnlyrepository;
    private readonly IUserWriteOnly _userWriteOnlyrepository = userWriteOnlyrepository;

    public async Task<Result<MessageResult>> ChangePasswordAsync(RequestChangePassword request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(ChangePasswordAsync)}.");

            var loggedUser = await _loggedUser.GetLoggedUserAsync();

            Validate(request, loggedUser);

            var user = await _userReadOnlyrepository.RecoverByIdAsync(loggedUser.Id);

            user.Password = _passwordEncryptor.Encrypt(request.NewPassword);

            _userWriteOnlyrepository.Update(user);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Senha atualizada com sucesso"));

            _logger.Information($"Fim {nameof(ChangePasswordAsync)}.");
        }
        catch (ValidationErrorsException ex)
        {
            var errorMessage = $"Ocorreram erros de validação: {string.Concat(string.Join(", ", ex.ErrorMessages), ".")}";

            _logger.Error(ex, errorMessage);

            output.Failure(ex.ErrorMessages);
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format("Algo deu errado: {0}", ex.Message);

            _logger.Error(ex, errorMessage);

            output.Failure(new List<string>() { errorMessage });
        }

        return output;
    }

    private void Validate(RequestChangePassword request, Domain.Entities.User user)
    {
        _logger.Information($"Início {nameof(Validate)}.");

        var validator = new ChangePasswordValidator();
        var result = validator.Validate(request);

        var currentPasswordEncrypted = _passwordEncryptor.Encrypt(request.CurrentPassword);

        if (!user.Password.Equals(currentPasswordEncrypted))
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure("currentPassword", ErrorsMessages.InvalidCurrentPassword));
        }

        if (!result.IsValid)
        {
            var mensagens = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ValidationErrorsException(mensagens);
        }
    }
}
