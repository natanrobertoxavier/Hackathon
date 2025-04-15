using Doctor.Application.Mapping;
using Doctor.Application.Services.Doctor;
using Doctor.Communication.Request;
using Doctor.Communication.Response;
using Doctor.Domain.ModelServices;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.ServiceDay;
using Doctor.Domain.Services;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;
using System.Globalization;

namespace Doctor.Application.UseCase.ServiceDay.Delete;

public class DeleteUseCase(
    IServiceDayWriteOnly serviceDayWriteOnlyrepository,
    IConsultationServiceApi consultationServiceApi,
    IWorkUnit workUnit,
    ILoggedDoctor loggedDoctor,
    ILogger logger) : IDeleteUseCase
{
    private readonly IServiceDayWriteOnly _serviceDayWriteOnlyrepository = serviceDayWriteOnlyrepository;
    private readonly IConsultationServiceApi _consultationServiceApi = consultationServiceApi;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedDoctor _loggedDoctor = loggedDoctor;
    private readonly ILogger _logger = logger;
    public async Task<Communication.Response.Result<MessageResult>> DeleteServiceDayAsync(RequestDeleteServiceDay request)
    {
        var output = new Communication.Response.Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(DeleteServiceDayAsync)}.");

            var doctor = _loggedDoctor.GetLoggedDoctor();

            await Validate(doctor.Id, request);

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

    private async Task Validate(Guid doctorId, RequestDeleteServiceDay request)
    {
        var validationResult = ValidateRequest(request);

        var consultations = await _consultationServiceApi.RecoverConsultationByDoctorIdAsync(doctorId);

        ValidateConsultations(request, consultations, validationResult);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }

    private static FluentValidation.Results.ValidationResult ValidateRequest(RequestDeleteServiceDay request)
    {
        var serviceDayValidator = new DeleteValidator();
        return serviceDayValidator.Validate(request);
    }

    private static void ValidateConsultations(
        RequestDeleteServiceDay request,
        Domain.ModelServices.Result<IEnumerable<ConsultationResult>> consultations,
        FluentValidation.Results.ValidationResult validationResult)
    {
        if (!consultations.Success)
        {
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                "consultation", ErrorsMessages.ErrorCallingConsultationApi));
            return;
        }

        if (consultations.Data == null || !consultations.Data.Any())
            return;

        var (hasActiveConsultations, days) = CheckActiveConsultations(request, consultations.Data);

        if (hasActiveConsultations)
        {
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                "consultation",
                $"Médico possui consultas confirmadas nos dias: {string.Join(", ", days)}"));
        }
    }

    private static (bool hasActiveConsultations, List<string> daysWithConsultations) CheckActiveConsultations(
        RequestDeleteServiceDay request,
        IEnumerable<Domain.ModelServices.ConsultationResult> consultations)
    {
        var consultationDays = consultations
            .Select(x => x.ConsultationDate.DayOfWeek)
            .ToHashSet();

        var xpto = request.Days
            .Where(day => consultationDays.Contains(day.Day))
            .ToList();

        var daysWithConsultations = request.Days
            .Where(day => consultationDays.Contains(day.Day))
            .Select(day => CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(day.Day))
            .ToList();

        return (daysWithConsultations.Any(), daysWithConsultations);
    }
}
