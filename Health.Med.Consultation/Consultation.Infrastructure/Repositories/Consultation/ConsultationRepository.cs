using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Consultation.Infrastructure.Repositories.Consultation;

public class ConsultationRepository(HealthMedContext context) : IConsultationReadOnly, IConsultationWriteOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.Consultation consultation) =>
        await _context.AddAsync(consultation);

    public async Task<bool> ThereIsConsultationForDoctor(Guid id, DateTime consultationDate) =>
        await _context.Consultations
        .AsNoTracking()
        .AnyAsync(c => c.DoctorId == id && c.ConsultationDate == consultationDate);

    public async Task<bool> ThereIsConsultationAsync(Guid id, Guid doctorId) =>
        await _context.Consultations
        .AsNoTracking()
        .AnyAsync(c => c.Id == id && c.DoctorId == doctorId);

    public async Task<bool> ThereIsConsultationForClient(Guid id, DateTime consultationDate) =>
        await _context.Consultations
        .AsNoTracking()
        .AnyAsync(c => c.ClientId == id && c.ConsultationDate == consultationDate);

    public async Task<Guid> GetIdByDateTimeAndDoctorAsync(DateTime dateTime, Guid doctorId) =>
        await _context.Consultations
            .AsNoTracking()
            .Where(c => c.ConsultationDate == dateTime && c.DoctorId == doctorId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

    public async Task AcceptConsultationAsync(Guid consultationId, DateTime date)
    {
        var consultation = await _context.Consultations.FindAsync(consultationId);
        if (consultation is not null)
        {
            consultation.Confirmed = true;
            consultation.ConfirmatonDate = date;
        }
    }

    public async Task RefuseConsultationAsync(Guid consultationId, DateTime date)
    {
        var consultation = await _context.Consultations.FindAsync(consultationId);
        if (consultation is not null)
        {
            consultation.Confirmed = false;
            consultation.ConfirmatonDate = date;
        }
    }
}
