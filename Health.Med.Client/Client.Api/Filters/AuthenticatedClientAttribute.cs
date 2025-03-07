using Health.Med.Exceptions.ExceptionBase;
using Health.Med.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TokenService.Manager.Controller;
using Client.Domain.Repositories.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Client.Api.Filters;
public class AuthenticatedClientAttribute(
    TokenController tokenController,
    IClientReadOnly clientReadOnlyrepository) : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly TokenController _tokenController = tokenController;
    private readonly IClientReadOnly _clientReadOnlyrepository = clientReadOnlyrepository;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenInRequest(context);
            var clientEmail = _tokenController.RecoverEmail(token);

            var client = await _clientReadOnlyrepository.RecoverByEmailAsync(clientEmail);

            if (client?.Id == Guid.Empty)
            {
                throw new ValidationException("Cliente não localizado para o token informado");
            }
        }
        catch (SecurityTokenExpiredException)
        {
            ExpiredToken(context);
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

    private static void UserWithoutPermission(AuthorizationFilterContext context)
    {
        context.Result = new UnauthorizedObjectResult(new Communication.Response.ResponseError(ErrorsMessages.ClientWithoutPermission));
    }
}