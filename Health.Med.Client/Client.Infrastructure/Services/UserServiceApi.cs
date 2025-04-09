using Client.Domain.ModelServices;
using Client.Domain.Services;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Client.Infrastructure.Services;
public class UserServiceApi(
    IHttpClientFactory httpClientFactory,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor) : Base, IUserServiceApi
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<UserResult>> RecoverByEmailAsync(string email)
    {
        _logger.Information($"{nameof(RecoverByEmailAsync)} - Iniciando a chamada para API de usuários. Usuário: {email}.");

        var output = new Result<UserResult>();

        try
        {
            var client = _httpClientFactory.CreateClient("UserApi");

            var uri = string.Format("/api/v1/user/{0}", email);

            var authorization = GetTokenInRequest();

            if (!string.IsNullOrEmpty(authorization))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
            }
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var responseApi = DeserializeResponseObject<Result<UserResult>>(content);

                _logger.Information($"{nameof(RecoverByEmailAsync)} - Encerrando chamada para API de usuários. Usuário: {email}.");

                return responseApi;
            }

            var failMessage = $"{nameof(RecoverByEmailAsync)} - Ocorreu um erro ao chamar a API de usuários. StatusCode: {response.StatusCode}";

            _logger.Error(failMessage);

            return output.Failure(failMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(RecoverByEmailAsync)} - Ocorreu um erro ao chamar a API de usuários. Erro: {ex.Message}";

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

