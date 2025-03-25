using Doctor.Application.Extensions;
using Doctor.Application.Mapping;
using Doctor.Application.Services.User;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.Specialty;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;

namespace Doctor.Application.UseCase.Specialty.Register;
public class RegisterUseCase(
    ISpecialtyReadOnly specialtyReadOnlyrepository,
    ISpecialtyWriteOnly specialtyWriteOnlyrepository,
    IWorkUnit workUnit,
    ILoggedUser loggedUser,
    ILogger logger) : IRegisterUseCase
{
    private readonly ISpecialtyReadOnly _specialtyReadOnlyrepository = specialtyReadOnlyrepository;
    private readonly ISpecialtyWriteOnly _specialtyWriteOnlyrepository = specialtyWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedUser _loggedUser = loggedUser;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterSpecialtyAsync(RequestRegisterSpecialty request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(RegisterSpecialtyAsync)}. Especialidade: {request.Description}.");

            var standardDescription = request.Description.NormalizeText();

            await Validate(request, standardDescription);

            var userId = _loggedUser.GetLoggedUserId();

            await _specialtyWriteOnlyrepository.AddAsync(request.ToEntity(standardDescription, userId));

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

            _logger.Information($"Fim {nameof(RegisterSpecialtyAsync)}. Especialidade: {request.Description}.");
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

    private async Task Validate(RequestRegisterSpecialty request, string standardDescription)
    {
        _logger.Information($"Início {nameof(Validate)}. Especialidade: {request.Description}.");

        var specialtyValidator = new RegisterValidator();
        var validationResult = specialtyValidator.Validate(request);

        var thereIsWithDescription = await _specialtyReadOnlyrepository.RecoverByStandardDescriptionAsync(standardDescription);

        if (thereIsWithDescription?.Id != Guid.Empty)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("specialty", ErrorsMessages.SpecialtyAlreadyRegistered));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}