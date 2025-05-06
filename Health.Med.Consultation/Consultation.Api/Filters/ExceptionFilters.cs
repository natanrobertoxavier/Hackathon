using Consultation.Communication.Response;
using Health.Med.Exceptions;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Consultation.Api.Filters;

public class ExceptionFilters : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is HealthMedException)
        {
            ProccessHealthMedException(context);
        }
        else
        {
            ThrowUnknowError(context);
        }
    }

    private static void ProccessHealthMedException(ExceptionContext context)
    {
        if (context.Exception is ValidationErrorsException)
        {
            ThrowValidationErros(context);
        }
        else if (context.Exception is InvalidLoginException)
        {
            ThrowLoginErrors(context);
        };
    }

    private static void ThrowValidationErros(ExceptionContext context)
    {
        var validationErrorException = context.Exception as ValidationErrorsException;

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Result = new ObjectResult(new ResponseError(validationErrorException.ErrorMessages));
    }

    private static void ThrowLoginErrors(ExceptionContext context)
    {
        var loginError = context.Exception as InvalidLoginException;

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Result = new ObjectResult(new ResponseError(loginError.Message));
    }

    private static void ThrowUnknowError(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new ObjectResult(new ResponseError(ErrorsMessages.UnknowError));
    }
}
