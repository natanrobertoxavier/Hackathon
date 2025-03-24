using Consultation.Application.DTO;

namespace Consultation.Application.Services.LoggedClientService;

public interface ILoggedClient
{
    Guid GetLoggedClientId();
    LoggedUserDto GetLoggedClient();
}
