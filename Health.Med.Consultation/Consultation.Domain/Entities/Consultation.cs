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
        DateTime consultationDate,
        string reason = "",
        bool confirmed = false)
    {
        ClientId = clientId;
        DoctorId = doctorId;
        ConsultationDate = consultationDate;
        Reason = reason;
        Confirmed = confirmed;
    }

    public Consultation()
    {
    }

    public Guid ClientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ConsultationDate { get; set; }
    public bool Confirmed { get; set; } = false;
    public DateTime? ConfirmatonDate { get; set; } = null;
    public string Reason { get; set; }
}
