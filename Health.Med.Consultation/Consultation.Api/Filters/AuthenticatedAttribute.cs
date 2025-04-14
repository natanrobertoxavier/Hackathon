using Health.Med.Exceptions.ExceptionBase;
using Health.Med.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TokenService.Manager.Controller;
using Microsoft.IdentityModel.Tokens;
using Consultation.Domain.Services;

namespace Consultation.Api.Filters;

public class AuthenticatedAttribute(
    TokenController tokenController,
    IClientServiceApi clientServiceApi,
    IUserServiceApi userServiceApi,
    IDoctorServiceApi doctorServiceApi) : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly TokenController _tokenController = tokenController;
    private readonly IClientServiceApi _clientServiceApi = clientServiceApi;
    private readonly IUserServiceApi _userServiceApi = userServiceApi;
    private readonly IDoctorServiceApi _doctorServiceApi = doctorServiceApi;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenInRequest(context);
            var email = _tokenController.RecoverEmail(token);

            var client = await _clientServiceApi.RecoverBasicInformationByEmailAsync(email);

            if (client.Success)
            {
                context.HttpContext.Items["AuthenticatedClient"] = client.Data;
                return;
            }

            var user = await _userServiceApi.RecoverByEmailAsync(email);
            if (user.Success)
            {
                context.HttpContext.Items["AuthenticatedUser"] = user.Data;
                return;
            }

            var doctor = await _doctorServiceApi.RecoverByEmailAsync(email);
            if (doctor.Success)
            {
                context.HttpContext.Items["AuthenticatedDoctor"] = doctor.Data;
                return;
            }

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

    private static string TokenInRequest(AuthorizationFilterContext context)
    {
        var authorization = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            throw new HealthMedException(string.Empty);
        }

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
