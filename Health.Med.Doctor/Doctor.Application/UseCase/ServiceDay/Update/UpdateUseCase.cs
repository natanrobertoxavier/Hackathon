using Doctor.Application.UseCase.ServiceDay.Delete;
using Doctor.Application.UseCase.ServiceDay.Register;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;

namespace Doctor.Application.UseCase.ServiceDay.Update;

public class UpdateUseCase(
    IDeleteUseCase deleteUseCase,
    IRegisterUseCase registerUseCase,
    ILogger logger) : IUpdateUseCase
{
    private readonly IDeleteUseCase _deleteUseCase = deleteUseCase;
    private readonly IRegisterUseCase _registerUseCase = registerUseCase;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> UpdateServiceDayAsync(RequestServiceDay request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(UpdateServiceDayAsync)}.");

            Validate(request);

            await DeleteExistingDays(request.ServiceDays.Select(d => d.Day).ToList());

            await RegisterNewDays(request);

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

    private async Task DeleteExistingDays(IEnumerable<DayOfWeek> request)
    {
        var methodName = nameof(RegisterNewDays);
        _logger.Information($"Início {methodName}.");

        var requestDeleteDays = request.Select(day => new DaysToRemove(day)).ToList();

        var resultDelete = await _deleteUseCase.DeleteServiceDayAsync(new RequestDeleteServiceDay(requestDeleteDays));

        if (!resultDelete.IsSuccess())
        {
            var errorMessage = $"{string.Concat(string.Join(", ", resultDelete.Errors), ".")}";
            _logger.Error($"{methodName} {errorMessage}");
            throw new ValidationErrorsException(new List<string>() { errorMessage });
        }

        _logger.Information($"Fim {methodName}.");
    }

    private async Task RegisterNewDays(RequestServiceDay request)
    {
        var methodName = nameof(RegisterNewDays);
        _logger.Information($"Início {methodName}.");

        var resultRegister = await _registerUseCase.RegisterServiceDayAsync(new RequestServiceDay(request.ServiceDays));

        if (!resultRegister.IsSuccess())
        {
            var errorMessage = $"Erro ao incluir os novos dias: {string.Concat(string.Join(", ", resultRegister.Errors), ".")}";
            _logger.Error($"{methodName} {errorMessage}");
            throw new Exception(errorMessage);
        }

        _logger.Information($"Fim {methodName}.");
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