using Consultation.Application.DTO;
using Health.Med.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Http;

namespace Consultation.Application.Services.LoggedClientService;

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

    public LoggedUserDto GetLoggedClient()
    {
        if (_httpContextAccessor.HttpContext?.Items["AuthenticatedClient"]
            is not Domain.ModelServices.ClientBasicInformationResult client)
        {
            throw new HealthMedException("Cliente não localizado");
        }

        return new LoggedUserDto(
            client.Id, 
            client.PreferredName, 
            client.Email);
    }
}
