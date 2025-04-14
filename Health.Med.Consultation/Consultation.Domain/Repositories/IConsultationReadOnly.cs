namespace Consultation.Domain.Repositories;

public interface IConsultationReadOnly
{
    Task<bool> ThereIsConsultationForDoctor(Guid id, DateTime consultationDate);
    Task<bool> ThereIsConsultationAsync(Guid id, Guid doctorId);
    Task<bool> ThereIsConsultationForClient(Guid id, DateTime consultationDate);
    Task<Guid> GetIdByDateTimeAndDoctorAsync(DateTime dateTime, Guid doctorId);
}
