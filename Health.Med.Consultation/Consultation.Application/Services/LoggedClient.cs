using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;

namespace Consultation.Application.Services;

public class LoggedClient(IHttpContextAccessor httpContextAccessor) : ILoggedClient
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetLoggedClientId()
    {
        if (_httpContextAccessor.HttpContext?.Items["AuthenticatedClient"]
            is not Domain.ModelServices.ClientBasicInformationResult client)
        {
            throw new HealthMedException("Cliente não localizado");
        }

        return client.Id;
    }
}
