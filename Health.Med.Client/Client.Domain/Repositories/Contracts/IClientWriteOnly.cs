namespace Client.Domain.Repositories.Contracts;

public interface IClientWriteOnly
{
    Task AddAsync(Entities.Client user);
    void Update(Entities.Client user);
}
