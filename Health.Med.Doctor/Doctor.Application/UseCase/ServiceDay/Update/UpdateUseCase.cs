using Doctor.Application.Services.Doctor;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories.Contracts.ServiceDay;
using Doctor.Domain.Repositories;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using Doctor.Application.Mapping;

namespace Doctor.Application.UseCase.ServiceDay.Update;

public class UpdateUseCase(
    IServiceDayWriteOnly serviceDayWriteOnlyrepository,
    IWorkUnit workUnit,
    ILoggedDoctor loggedDoctor,
    ILogger logger) : IUpdateUseCase
{
    private readonly IServiceDayWriteOnly _serviceDayWriteOnlyrepository = serviceDayWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedDoctor _loggedDoctor = loggedDoctor;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> UpdateServiceDayAsync(RequestServiceDay request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(UpdateServiceDayAsync)}.");

            Validate(request);

            var doctor = await _loggedDoctor.GetLoggedDoctorAsync();

            var serviceDays = request.ToListEntity(doctor.Id);

            _serviceDayWriteOnlyrepository.Update(serviceDays);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Atualização realizada com sucesso"));

            _logger.Information($"Fim {nameof(UpdateServiceDayAsync)}.");
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

        var serviceDayValidator = new UpdateValidator();
        var validationResult = serviceDayValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}