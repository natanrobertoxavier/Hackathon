namespace Consultation.Application.Services;

public interface ILoggedClient
{
    Task<Guid> GetLoggedClientIdAsync();
}
