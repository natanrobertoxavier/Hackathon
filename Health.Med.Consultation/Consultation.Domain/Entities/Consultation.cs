namespace Consultation.Domain.Entities;

public class Consultation : BaseEntity
{
    public Consultation(
        Guid id,
        DateTime registrationDate,
        Guid clientId,
        Guid doctorId,
        DateTime consultationDate) : base(id, registrationDate)
    {
        ClientId = clientId;
        DoctorId = doctorId;
        ConsultationDate = consultationDate;
    }

    public Consultation(
        Guid clientId,
        Guid doctorId,
        DateTime consultationDate)
    {
        ClientId = clientId;
        DoctorId = doctorId;
        ConsultationDate = consultationDate;
    }

    public Consultation()
    {
    }

    public Guid ClientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ConsultationDate { get; set; }
}
