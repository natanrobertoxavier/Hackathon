using Doctor.Domain.ModelServices;
using Doctor.Domain.Services;
using Health.Med.Exceptions.ExceptionBase;
using Health.Med.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Doctor.Infrastructure.Services;

public class ConsultationServiceApi(
    IHttpClientFactory httpClientFactory,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor) : Base, IConsultationServiceApi
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<Result<IEnumerable<ConsultationResult>>> RecoverConsultationByDoctorIdAsync(Guid doctorId)
    {
        _logger.Information($"{nameof(RecoverConsultationByDoctorIdAsync)} - Iniciando a chamada para API de consultas. Médico: {doctorId}.");

        var output = new Result<IEnumerable<ConsultationResult>>();

        try
        {
            var client = _httpClientFactory.CreateClient("ConsultationApi");

            var uri = string.Format("/api/v1/consultation/confirmed/{0}", doctorId);

            var authorization = GetTokenInRequest();

            if (!string.IsNullOrEmpty(authorization))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
            }

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var responseApi = DeserializeResponseObject<Result<IEnumerable<ConsultationResult>>>(content);

                _logger.Information($"{nameof(RecoverConsultationByDoctorIdAsync)} - Encerrando chamada para API de consultas. Médico: {doctorId}.");

                return responseApi;
            }

            var failMessage = $"{nameof(RecoverConsultationByDoctorIdAsync)} - Ocorreu um erro ao chamar a API de consultas. StatusCode: {response.StatusCode}";

            _logger.Error(failMessage);

            return output.Failure(failMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(RecoverConsultationByDoctorIdAsync)} - Ocorreu um erro ao chamar a API de consultas. Erro: {ex.Message}";

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