using Doctor.Application.Mapping;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Application.UseCase.Register;
public class RegisterDoctorUseCase(
    IDoctorWriteOnly doctorWriteOnlyrepository,
    IDoctorReadOnly doctorReadOnlyrepository,
    PasswordEncryptor passwordEncryptor,
    ILogger logger) : IRegisterDoctorUseCase
{
    private IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private IDoctorWriteOnly _doctorWriteOnlyrepository = doctorWriteOnlyrepository;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterDoctorAsync(RequestRegisterDoctor request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Start {nameof(RegisterDoctorAsync)}. Doctor: {request.CR}.");

            await Validate(request);

            var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

            var doctor = request.ToEntity(encryptedPassword);

            await _doctorWriteOnlyrepository.AddAsync(doctor);

            _logger.Information($"End {nameof(RegisterDoctorAsync)}. Doctor: {request.CR}.");

            output.Succeeded(new MessageResult("Cadastro em processamento."));
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

    private async Task Validate(RequestRegisterDoctor request)
    {
        _logger.Information($"Start {nameof(Validate)}. Doctor: {request.CR}.");

        var registerUserValidator = new RegisterUserValidator();
        var validationResult = registerUserValidator.Validate(request);

        //Adicionara validação de email e de CR já existente
        //var thereIsUserWithEmail = await _userQueryServiceApi.ThereIsUserWithEmailAsync(request.Email);

        //if (thereIsUserWithEmail.IsSuccess && thereIsUserWithEmail.Data.ThereIsUser)
        //{
        //    validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ErrorsMessages.EmailAlreadyRegistered));
        //}
        //else if (!thereIsUserWithEmail.IsSuccess)
        //{
        //    validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("responseApi", $"{thereIsUserWithEmail.Error}"));
        //}

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}
