using Health.Med.Exceptions.ExceptionBase;
using Health.Med.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TokenService.Manager.Controller;
using Microsoft.IdentityModel.Tokens;
using Consultation.Domain.Services;
using System.Reflection;

namespace Consultation.Api.Filters;

public class AuthenticatedAttribute(
    TokenController tokenController,
    IClientServiceApi clientServiceApi,
    IUserServiceApi userServiceApi,
    IDoctorServiceApi doctorServiceApi,
    Serilog.ILogger logger) : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly TokenController _tokenController = tokenController;
    private readonly IClientServiceApi _clientServiceApi = clientServiceApi;
    private readonly IUserServiceApi _userServiceApi = userServiceApi;
    private readonly IDoctorServiceApi _doctorServiceApi = doctorServiceApi;
    private readonly Serilog.ILogger _logger = logger;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var methodName = nameof(OnAuthorizationAsync);

        try
        {
            _logger.Information($"{methodName} - Iniciando autenticação do usuário logado.");
            var token = TokenInRequest(context);

            _logger.Information($"{methodName} - Recuperando e-mail do token.");
            var email = _tokenController.RecoverEmail(token);

            _logger.Information($"{methodName} - Iniciando chamada a API de clientes.");
            var client = await _clientServiceApi.RecoverBasicInformationByEmailAsync(email);
            _logger.Information($"{methodName} - Sucesso na chamada da API de clientes?: {client.Success}.");
            if (client.Success)
            {
                context.HttpContext.Items["AuthenticatedClient"] = client.Data;
                return;
            }
            else
                _logger.Error(client.Error);

                _logger.Information($"{methodName} - Iniciando chamada a API de usuários.");
            var user = await _userServiceApi.RecoverByEmailAsync(email);
            _logger.Information($"{methodName} - Sucesso na chamada da API de usuários?: {user.Success}.");
            if (user.Success)
            {
                context.HttpContext.Items["AuthenticatedUser"] = user.Data;
                return;
            }
            else
                _logger.Error(user.Error);

            _logger.Information($"{methodName} - Iniciando chamada a API de médicos.");
            var doctor = await _doctorServiceApi.RecoverByEmailAsync(email);
            _logger.Information($"{methodName} - Sucesso na chamada da API de médicos?: {doctor.Success}.");
            if (doctor.Success)
            {
                context.HttpContext.Items["AuthenticatedDoctor"] = doctor.Data;
                return;
            }
            else
                _logger.Error(doctor.Error);

            throw new ValidationException($"Ocorreu um erro ao autenticar o usuário pelo token.");
        }
        catch (SecurityTokenExpiredException)
        {
            ExpiredToken(context);
        }
        catch (ValidationException ex)
        {
            ApiCalledError(context, ex.Message);
        }
        catch
        {
            UserWithoutPermission(context);
        }
    }

    private string TokenInRequest(AuthorizationFilterContext context)
    {
        var methodName = nameof(TokenInRequest);

        _logger.Information($"{methodName} - Obtendo token da requisição.");

        var authorization = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            _logger.Information($"{methodName} - Token da request está nulo.");
            throw new HealthMedException(string.Empty);
        }

        _logger.Information($"{methodName} - Fim obter token da requisição.");
        return authorization["Bearer".Length..].Trim();
    }

    private static void ExpiredToken(AuthorizationFilterContext context)
    {
        context.Result = new UnauthorizedObjectResult(new Communication.Response.ResponseError(ErrorsMessages.ExpiredToken));
    }

    private static void ApiCalledError(AuthorizationFilterContext context, string errorMessage)
    {
        context.Result = new UnauthorizedObjectResult(new Communication.Response.ResponseError(errorMessage));
    }

    private static void UserWithoutPermission(AuthorizationFilterContext context)
    {
        context.Result = new UnauthorizedObjectResult(new Communication.Response.ResponseError(ErrorsMessages.UserWithoutPermission));
    }
}
