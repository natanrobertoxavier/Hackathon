namespace Client.Application.Services;

public interface ILoggedClient
{
    Task<Domain.Entities.Client> GetLoggedClientAsync();
}
