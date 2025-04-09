namespace User.Application.Services;

public interface ILoggedUser
{
    Task<Domain.Entities.User> GetLoggedUserAsync();
}
