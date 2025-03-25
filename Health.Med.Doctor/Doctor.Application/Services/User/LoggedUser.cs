using Doctor.Domain.ModelServices;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;

namespace Doctor.Application.Services.User;

public class LoggedUser(IHttpContextAccessor httpContextAccessor) : ILoggedUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetLoggedUserId()
    {
        if (_httpContextAccessor.HttpContext?.Items["AuthenticatedUser"]
            is not UserResult user)
        {
            throw new HealthMedException("Usuário não localizado");
        }

        return user.Id;
    }
}