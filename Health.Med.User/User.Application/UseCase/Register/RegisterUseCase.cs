using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;
using User.Application.Mapping;
using User.Communication.Request;
using User.Communication.Response;
using User.Domain.Repositories;
using User.Domain.Repositories.Contracts;

namespace User.Application.UseCase.Register;

public class RegisterUseCase(
    IUserReadOnly userReadOnlyrepository,
    IUserWriteOnly userWriteOnlyrepository,
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    ILogger logger) : IRegisterUseCase
{
    private readonly IUserReadOnly _userReadOnlyrepository = userReadOnlyrepository;
    private readonly IUserWriteOnly _userWriteOnlyrepository = userWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterUserAsync(RequestRegisterUser request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(RegisterUserAsync)}.");

            await Validate(request);

            var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

            var user = request.ToEntity(encryptedPassword);

            await _userWriteOnlyrepository.AddAsync(user);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

            _logger.Information($"Fim {nameof(RegisterUserAsync)}.");
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

    private async Task Validate(RequestRegisterUser request)
    {
        _logger.Information($"Início {nameof(Validate)}.");

        var userValidator = new RegisterValidator();
        var validationResult = userValidator.Validate(request);

        var thereIsWithEmail = await _userReadOnlyrepository.RecoverByEmailAsync(request.Email);

        if (thereIsWithEmail?.Id != Guid.Empty)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ErrorsMessages.EmailAlreadyRegistered));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}
