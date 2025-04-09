using Client.Application.Services;
using Client.Communication.Request;
using Client.Communication.Response;
using Client.Domain.Repositories;
using Client.Domain.Repositories.Contracts;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Client.Application.UseCase.ChangePassword;

public class ChangePasswordUseCase(
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    ILogger logger,
    ILoggedClient loggedClient,
    IClientReadOnly clientReadOnlyrepository,
    IClientWriteOnly clientWriteOnlyrepository) : IChangePasswordUseCase
{
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;
    private readonly ILoggedClient _loggedClient = loggedClient;
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
    private readonly IClientWriteOnly _clientWriteOnlyrepository = clientWriteOnlyrepository;

    public async Task<Result<MessageResult>> ChangePasswordAsync(RequestChangePassword request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(ChangePasswordAsync)}.");

            var loggedClient = await _loggedClient.GetLoggedClientAsync();

            Validate(request, loggedClient);

            var client = await _clientReadOnlyrepository.RecoverByIdAsync(loggedClient.Id);

            client.Password = _passwordEncryptor.Encrypt(request.NewPassword);

            _clientWriteOnlyrepository.Update(client);

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

    private void Validate(RequestChangePassword request, Domain.Entities.Client client)
    {
        _logger.Information($"Início {nameof(Validate)}.");

        var validator = new ChangePasswordValidator();
        var result = validator.Validate(request);

        var currentPasswordEncrypted = _passwordEncryptor.Encrypt(request.CurrentPassword);

        if (!client.Password.Equals(currentPasswordEncrypted))
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
