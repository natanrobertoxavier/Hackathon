using Consultation.Domain.ModelServices;
using Consultation.Domain.Services;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Consultation.Infrastructure.Services;

public class ClientServiceApi(
    IHttpClientFactory httpClientFactory,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor) : Base, IClientServiceApi
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<ClientBasicInformationResult>> RecoverBasicInformationByClientIdAsync(Guid clientId)
    {
        _logger.Information($"{nameof(RecoverBasicInformationByClientIdAsync)} - Iniciando a chamada para API de clientes. Cliente: {clientId}.");

        var output = new Result<ClientBasicInformationResult>();

        try
        {
            var client = _httpClientFactory.CreateClient("ClientApi");

            var uri = string.Format("/api/v1/client/basic-info/id/{0}", clientId);

            var authorization = GetTokenInRequest();

            if (!string.IsNullOrEmpty(authorization))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
            }

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var responseApi = DeserializeResponseObject<Result<ClientBasicInformationResult>>(content);

                _logger.Information($"{nameof(RecoverBasicInformationByClientIdAsync)} - Encerrando chamada para API de clientes. Cliente: {clientId}.");

                return responseApi;
            }

            var failMessage = $"{nameof(RecoverBasicInformationByClientIdAsync)} - Ocorreu um erro ao chamar a API de clientes. StatusCode: {response.StatusCode}";

            _logger.Error(failMessage);

            return output.Failure(failMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(RecoverBasicInformationByEmailAsync)} - Ocorreu um erro ao chamar a API de clientes. Erro: {ex.Message}";

            _logger.Error(errorMessage);

            return output.Failure(errorMessage);
        }
    }

    public async Task<Result<ClientBasicInformationResult>> RecoverBasicInformationByEmailAsync(string email)
    {
        _logger.Information($"{nameof(RecoverBasicInformationByEmailAsync)} - Iniciando a chamada para API de clientes. Cliente: {email}.");

        var output = new Result<ClientBasicInformationResult>();

        try
        {
            var client = _httpClientFactory.CreateClient("ClientApi");

            var uri = string.Format("/api/v1/client/basic-info/{0}", email);

            var authorization = GetTokenInRequest();

            if (!string.IsNullOrEmpty(authorization))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
            }

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var responseApi = DeserializeResponseObject<Result<ClientBasicInformationResult>>(content);

                _logger.Information($"{nameof(RecoverBasicInformationByEmailAsync)} - Encerrando chamada para API de clientes. Cliente: {email}.");

                return responseApi;
            }

            var failMessage = $"{nameof(RecoverBasicInformationByEmailAsync)} - Ocorreu um erro ao chamar a API de clientes. StatusCode: {response.StatusCode}";

            _logger.Error(failMessage);

            return output.Failure(failMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(RecoverBasicInformationByEmailAsync)} - Ocorreu um erro ao chamar a API de clientes. Erro: {ex.Message}";

            _logger.Error(errorMessage);

            return output.Failure(errorMessage);
        }
    }

    private string GetTokenInRequest()
    {
        var authorization = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            throw new HealthMedException(ErrorsMessages.TokenNotFound);
        }

        return authorization["Bearer".Length..].Trim();
    }
}

