using Consultation.Application.Extensions;
using Consultation.Application.Mapping;
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.UseCase.SendEmailClient;
using Consultation.Communication.Request;
using Consultation.Communication.Response;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Serilog;

namespace Consultation.Application.UseCase.Consultation.Register;

public class RegisterUseCase(
    ILoggedClient loggedClient,
    IConsultationReadOnly consultationReadOnlyrepository,
    IConsultationWriteOnly consultationWriteOnlyrepository,
    IWorkUnit workUnit,
    ISendEmailClientUseCase sendEmailClientUseCase,
    ILogger logger) : IRegisterUseCase
{
    private readonly ILoggedClient _loggedClient = loggedClient;
    private readonly IConsultationReadOnly _consultationReadOnlyrepository = consultationReadOnlyrepository;
    private readonly IConsultationWriteOnly _consultationWriteOnlyrepository = consultationWriteOnlyrepository;
    private readonly ISendEmailClientUseCase _sendEmailClientUseCase = sendEmailClientUseCase;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> RegisterConsultationAsync(RequestRegisterConsultation request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(RegisterConsultationAsync)}.");

            var clientId = _loggedClient.GetLoggedClientId();

            await Validate(request, clientId);

            await _consultationWriteOnlyrepository.AddAsync(request.ToEntity(clientId));

            await _workUnit.CommitAsync();

            await SendEmailAsync(request);

            output.Succeeded(new MessageResult("Cadastro realizado com sucesso"));

            _logger.Information($"Fim {nameof(RegisterConsultationAsync)}.");
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

    private async Task Validate(RequestRegisterConsultation request, Guid clientId)
    {
        _logger.Information($"Início {nameof(Validate)}.");

        var consultationValidator = new RegisterValidator();
        var validationResult = consultationValidator.Validate(request);

        var thereIsConsultationClient = await _consultationReadOnlyrepository
            .ThereIsConsultationForClient(clientId, request.ConsultationDate.TrimMilliseconds());

        if (thereIsConsultationClient)
            validationResult.Errors.Add(
                new FluentValidation.Results.ValidationFailure("client", string.Format(ErrorsMessages.ClientAlreadyConsultationSchedule), request.ConsultationDate));

        var thereIsConsultationDoctor = await _consultationReadOnlyrepository
            .ThereIsConsultationForDoctor(request.DoctorId, request.ConsultationDate.TrimMilliseconds());

        if (thereIsConsultationDoctor)
            validationResult.Errors.Add(
                new FluentValidation.Results.ValidationFailure("doctor", string.Format(ErrorsMessages.DoctorAlreadyConsultationSchedule), request.ConsultationDate));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }

    private async Task SendEmailAsync(RequestRegisterConsultation request)
    {
        _logger.Information($"Início {nameof(SendEmailAsync)}.");

        await _sendEmailClientUseCase.SendEmailClientAsync(request, Domain.Entities.Enum.TemplateEmailEnum.ConsultationSchedulingEmail);
    }
}
