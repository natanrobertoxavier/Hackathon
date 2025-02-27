using Client.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Repositories.Client;

public class ClientRepository(HealthMedContext context) : IClientReadOnly, IClientWriteOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.Client doctor) =>
        await _context.AddAsync(doctor);

    public void Update(Domain.Entities.Client doctor) =>
        _context.Users.Update(doctor);

    public async Task<Domain.Entities.Client> RecoverByEmailAsync(string email) =>
        await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.Email.Equals(email)) ??
        new Domain.Entities.Client();

    public async Task<IEnumerable<Domain.Entities.Client>> RecoverAllAsync(int skip, int pageSize) =>
        await _context.Users
        .AsNoTracking()
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();

    public async Task<Domain.Entities.Client> RecoverByEmailPasswordAsync(string email, string password) =>
        await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(
            d => d.Email.Equals(email) &&
            d.Password.Equals(password)
        ) ?? new Domain.Entities.Client();

    public async Task<Domain.Entities.Client> RecoverByIdAsync(Guid id) =>
        await _context.Users
        .FirstOrDefaultAsync(d => d.Id.Equals(id)) ?? new Domain.Entities.Client();

    public async Task<Domain.Entities.Client> RecoverByCPFAsync(string cPF) =>
        await _context.Users
        .FirstOrDefaultAsync(d => d.CPF.Equals(cPF)) ?? new Domain.Entities.Client();
}