using Doctor.Persistence.Domain.Repositories;

namespace Doctor.Persistence.Infrastructure.Repositories;

public class WorkUnit(
    HealthMedContext context) : IDisposable, IWorkUnit
{
    private readonly HealthMedContext _context = context;
    private bool _disposed;

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }
}