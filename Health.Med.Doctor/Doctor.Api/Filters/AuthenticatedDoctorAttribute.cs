using Azure;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using TokenService.Manager.Controller;

namespace Doctor.Api.Filters;

public class AuthenticatedDoctorAttribute(
    TokenController tokenController,
    IDoctorReadOnly doctorReadOnlyrepository) : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly TokenController _tokenController = tokenController;
    private readonly IDoctorReadOnly _doctorReadOnlyrepository = doctorReadOnlyrepository;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenInRequest(context);
            var email = _tokenController.RecoverEmail(token);

            var doctor = await _doctorReadOnlyrepository.RecoverByEmailAsync(email);

            if (doctor?.Id == Guid.Empty)
            {
                throw new ValidationException("Médico não localizado para o token informado");
            }

            context.HttpContext.Items["AuthenticatedDoctor"] = doctor;
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
        context.Result = new UnauthorizedObjectResult(new Communication.Response.ResponseError(ErrorsMessages.DoctorWithoutPermission));
    }
}
