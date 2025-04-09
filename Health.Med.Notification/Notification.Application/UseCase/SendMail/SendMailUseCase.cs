using Serilog;
using Health.Med.Exceptions.ExceptionBase;
using Notification.Application.Mapping;
using Notification.Communication.Request;
using Notification.Communication.Response;
using Notification.Domain.Service;

namespace Notification.Application.UseCase.SendMail;
public class SendMailUseCase(
    IMailSender mailSender,
    ILogger logger) : ISendMailUseCase
{
    private readonly IMailSender _mailSender = mailSender;
    private readonly ILogger _logger = logger;

    public async Task<Result<MessageResult>> SendAsync(RequestSendMail request)
    {
        var output = new Result<MessageResult>();

        try
        {
            _logger.Information($"Início {nameof(SendAsync)}.");

            ValidateData(request);

            var entity = request.ToEntity();

            await _mailSender.SendAsync(entity);

            output.Succeeded(new MessageResult("E-mail enviado com sucesso"));

            _logger.Information($"Fim {nameof(SendAsync)}.");
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

    private static void ValidateData(RequestSendMail request)
    {
        var validator = new SendMailValidator(request.Recipients);

        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var messageError = result.Errors.Select(error => error.ErrorMessage).Distinct().ToList();
            throw new ValidationErrorsException(messageError);
        }
    }
}
