using Client.Application.Mapping;
using Client.Communication.Request;
using Client.Communication.Response;
using Client.Domain.Repositories;
using Client.Domain.Repositories.Contracts;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Client.Application.UseCase.Register;

public class RegisterUseCase(
    IClientReadOnly clientReadOnlyrepository,
    IClientWriteOnly clientWriteOnlyrepository,
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    ILogger logger) : IRegisterUseCase
{
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;
    private readonly IClientWriteOnly _clientWriteOnlyrepository = clientWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterClientAsync(RequestRegisterClient request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(RegisterClientAsync)}.");

            await Validate(request);

            var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

            var user = request.ToEntity(encryptedPassword);

            await _clientWriteOnlyrepository.AddAsync(user);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

            _logger.Information($"Fim {nameof(RegisterClientAsync)}.");
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

    private async Task Validate(RequestRegisterClient request)
    {
        _logger.Information($"Início {nameof(Validate)}.");

        var userValidator = new RegisterValidator();
        var validationResult = userValidator.Validate(request);

        var thereIsWithEmail = await _clientReadOnlyrepository.RecoverByEmailAsync(request.Email);

        if (thereIsWithEmail?.Id != Guid.Empty)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ErrorsMessages.EmailAlreadyRegistered));
        
        var thereIsWithCPF = await _clientReadOnlyrepository.RecoverByCPFAsync(request.CPF);

        if (thereIsWithCPF?.Id != Guid.Empty)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ErrorsMessages.CPFAlreadyRegistered));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}
