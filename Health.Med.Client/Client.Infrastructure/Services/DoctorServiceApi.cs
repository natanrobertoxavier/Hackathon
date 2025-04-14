using Client.Domain.ModelServices;
using Client.Domain.Services;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Client.Infrastructure.Services;

public class DoctorServiceApi(
    IHttpClientFactory httpClientFactory,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor) : Base, IDoctorServiceApi
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<DoctorResult>> RecoverByEmailAsync(string email)
    {
        _logger.Information($"{nameof(RecoverByEmailAsync)} - Iniciando a chamada para API de médicos. Médico: {email}.");

        var output = new Result<DoctorResult>();

        try
        {
            var client = _httpClientFactory.CreateClient("DoctorApi");

            var uri = string.Format("/api/v1/doctor/email/{0}", email);

            var authorization = GetTokenInRequest();

            if (!string.IsNullOrEmpty(authorization))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
            }

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode ||
                response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                var content = await response.Content.ReadAsStringAsync();

                var responseApi = DeserializeResponseObject<Result<DoctorResult>>(content);

                _logger.Information($"{nameof(RecoverByEmailAsync)} - Encerrando chamada para API de médicos. Médico: {email}.");

                return responseApi;
            }

            var failMessage = $"{nameof(RecoverByEmailAsync)} - Ocorreu um erro ao chamar a API de médicos. StatusCode: {response.StatusCode}";

            _logger.Error(failMessage);

            return output.Failure(failMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(RecoverByEmailAsync)} - Ocorreu um erro ao chamar a API de médicos. Erro: {ex.Message}";

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