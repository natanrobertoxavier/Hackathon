using Doctor.Domain.Services;
using Health.Med.Exceptions.ExceptionBase;
using Health.Med.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TokenService.Manager.Controller;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Api.Filters;

public class AuthenticatedAttribute(
    TokenController tokenController,
    IClientServiceApi clientServiceApi,
    IUserServiceApi userServiceApi) : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly TokenController _tokenController = tokenController;
    private readonly IClientServiceApi _clientServiceApi = clientServiceApi;
    private readonly IUserServiceApi _userServiceApi = userServiceApi;

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