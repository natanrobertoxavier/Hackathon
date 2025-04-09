using Doctor.Application.Services.Doctor;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Application.UseCase.Doctor.ChangePassword;

public class ChangePasswordUseCase(
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    ILogger logger,
    ILoggedDoctor loggedDoctor,
    IDoctorReadOnly doctorReadOnlyrepository,
    IDoctorWriteOnly doctorWriteOnlyrepository) : IChangePasswordUseCase
{
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly ILogger _logger = logger;
    private readonly ILoggedDoctor _loggedDoctor = loggedDoctor;
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;
    private readonly IDoctorWriteOnly _doctorWriteOnlyrepository = doctorWriteOnlyrepository;

    public async Task<Result<MessageResult>> ChangePasswordAsync(RequestChangePassword request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(ChangePasswordAsync)}.");

            var loggedDoctor = await _loggedDoctor.GetLoggedDoctorAsync();

            Validate(request, loggedDoctor);

            var doctor = await _doctorReadOnlyrepository.RecoverByIdAsync(loggedDoctor.Id);

            doctor.Password = _passwordEncryptor.Encrypt(request.NewPassword);

            _doctorWriteOnlyrepository.Update(doctor);

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

    private void Validate(RequestChangePassword request, Domain.Entities.Doctor doctor)
    {
        _logger.Information($"Início {nameof(Validate)}. Médico: {doctor.CR}.");

        var validator = new ChangePasswordValidator();
        var result = validator.Validate(request);

        var currentPasswordEncrypted = _passwordEncryptor.Encrypt(request.CurrentPassword);

        if (!doctor.Password.Equals(currentPasswordEncrypted))
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
