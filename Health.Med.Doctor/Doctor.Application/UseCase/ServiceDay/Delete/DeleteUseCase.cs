using Doctor.Application.Mapping;
using Doctor.Application.Services.Doctor;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.ServiceDay;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;

namespace Doctor.Application.UseCase.ServiceDay.Delete;

public class DeleteUseCase(
    IServiceDayWriteOnly serviceDayWriteOnlyrepository,
    IWorkUnit workUnit,
    ILoggedDoctor loggedDoctor,
    ILogger logger) : IDeleteUseCase
{
    private readonly IServiceDayWriteOnly _serviceDayWriteOnlyrepository = serviceDayWriteOnlyrepository;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedDoctor _loggedDoctor = loggedDoctor;
    private readonly ILogger _logger = logger;
    public async Task<Result<MessageResult>> DeleteServiceDayAsync(RequestDeleteServiceDay request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(DeleteServiceDayAsync)}.");

            //Validate(request);

            var doctor = await _loggedDoctor.GetLoggedDoctorAsync();

            var serviceDays = request.ToDeleteEntity();

            _serviceDayWriteOnlyrepository.Remove(doctor.Id, serviceDays);

            await _workUnit.CommitAsync();

            output.Succeeded(new MessageResult("Remoção feita com sucesso."));

            _logger.Information($"Fim {nameof(DeleteServiceDayAsync)}.");
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
}
