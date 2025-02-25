namespace User.Domain.Repositories.Contracts;

public interface IUserWriteOnly
{
    Task AddAsync(Entities.User user);
    void Update(Entities.User user);
}
