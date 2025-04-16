using Consultation.Domain.Entities;

namespace Consultation.Domain.Repositories;

public interface IConsultationReadOnly
{
    Task<bool> ThereIsConsultationForDoctor(Guid id, DateTime consultationDate);
    Task<DateTime> ThereIsConsultationAsync(Guid consultationId, Guid doctorId);
    Task<DateTime> ThereIsConsultationForClientAsync(Guid consultationId, Guid client);
    Task<bool> ThereIsConsultationForClient(Guid id, DateTime consultationDate);
    Task<Guid> GetIdByDateTimeAndDoctorAsync(DateTime dateTime, Guid doctorId);
    Task<Entities.Consultation> GetConsultationByIdAsync(Guid consultationId);
    Task<IEnumerable<Entities.Consultation>> GetConsultationByDoctorIdAsync(Guid doctorId);
}
