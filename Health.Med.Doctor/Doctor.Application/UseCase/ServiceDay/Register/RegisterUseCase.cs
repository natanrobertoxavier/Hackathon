using Doctor.Application.Mapping;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Doctor.Domain.Repositories;
using Health.Med.Exceptions.ExceptionBase;
using Health.Med.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;
using TokenService.Manager.Controller;
using Doctor.Application.Services.Doctor;
using Doctor.Domain.Repositories.Contracts.ServiceDay;

namespace Doctor.Application.UseCase.ServiceDay.Register;

public class RegisterUseCase(
    IServiceDayWriteOnly serviceDayWriteOnlyrepository,
    IWorkUnit workUnit,
    ILoggedDoctor loggedDoctor,
    ILogger logger) : IRegisterUseCase
{
    private readonly IServiceDayWriteOnly _serviceDayWriteOnlyrepository = serviceDayWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedDoctor _loggedDoctor = loggedDoctor;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterServiceDayAsync(RequestServiceDay request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(RegisterServiceDayAsync)}.");

            Validate(request);

            var doctor = _loggedDoctor.GetLoggedDoctor();

            var serviceDays = request.ToListEntity(doctor.Id);

            await _serviceDayWriteOnlyrepository.AddAsync(serviceDays);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

            _logger.Information($"Fim {nameof(RegisterServiceDayAsync)}.");
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

    private void Validate(RequestServiceDay request)
    {
        _logger.Information($"Início {nameof(Validate)}.");

        var serviceDayValidator = new RegisterValidator();
        var validationResult = serviceDayValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}