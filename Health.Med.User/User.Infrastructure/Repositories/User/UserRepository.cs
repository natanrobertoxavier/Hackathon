using Microsoft.EntityFrameworkCore;
using User.Domain.Repositories.Contracts;

namespace User.Infrastructure.Repositories.User;

public class UserRepository(HealthMedContext context) : IUserReadOnly, IUserWriteOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.User doctor) =>
        await _context.AddAsync(doctor);

    public void Update(Domain.Entities.User doctor) =>
        _context.Users.Update(doctor);

    public async Task<Domain.Entities.User> RecoverByEmailAsync(string email) =>
        await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.Email.Equals(email)) ??
        new Domain.Entities.User();

    public async Task<IEnumerable<Domain.Entities.User>> RecoverAllAsync(int skip, int pageSize) =>
        await _context.Users
        .AsNoTracking()
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();

    public async Task<Domain.Entities.User> RecoverByEmailPasswordAsync(string email, string password) =>
        await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(
            d => d.Email.Equals(email) &&
            d.Password.Equals(password)
        ) ?? new Domain.Entities.User();

    public async Task<Domain.Entities.User> RecoverByIdAsync(Guid id) =>
        await _context.Users
        .FirstOrDefaultAsync(d => d.Id.Equals(id)) ?? new Domain.Entities.User();
}