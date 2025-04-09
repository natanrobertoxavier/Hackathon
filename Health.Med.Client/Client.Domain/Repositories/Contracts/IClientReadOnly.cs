namespace Client.Domain.Repositories.Contracts;

public interface IClientReadOnly
{
    Task<Entities.Client> RecoverByEmailAsync(string email);
    Task<IEnumerable<Entities.Client>> RecoverAllAsync(int page, int pageSize);
    Task<Entities.Client> RecoverByEmailPasswordAsync(string email, string password);
    Task<Entities.Client> RecoverByIdAsync(Guid id);
    Task<Entities.Client> RecoverByCPFAsync(string cPF);
}
