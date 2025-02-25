using Doctor.Application.Mapping;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Application.UseCase.Register;
public class RegisterUseCase(
    IDoctorReadOnly doctorReadOnlyrepository,
    IDoctorWriteOnly doctorWriteOnlyrepository,
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    ILogger logger) : IRegisterUseCase
{
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly IDoctorWriteOnly _doctorWriteOnlyrepository = doctorWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterDoctorAsync(RequestRegisterDoctor request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(RegisterDoctorAsync)}. Médico: {request.CR}.");

            await Validate(request);

            var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

            var doctor = request.ToEntity(encryptedPassword);

            await _doctorWriteOnlyrepository.AddAsync(doctor);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

            _logger.Information($"Fim {nameof(RegisterDoctorAsync)}. Médico: {request.CR}.");
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
        _logger.Information($"Início {nameof(Validate)}. Médico: {request.CR}.");

        var doctorValidator = new RegisterValidator();
        var validationResult = doctorValidator.Validate(request);

        var thereIsWithEmail = await _doctorReadOnlyrepository.RecoverByEmailAsync(request.Email);

        if (thereIsWithEmail?.Id != Guid.Empty)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ErrorsMessages.EmailAlreadyRegistered));

        var thereIsWithCR = await _doctorReadOnlyrepository.RecoverByCRAsync(request.CR);

        if (thereIsWithCR?.Id != Guid.Empty)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("cr", ErrorsMessages.CRAlreadyRegistered));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}
