namespace Consultation.Communication.Request;

public class RequestRegisterConsultation
{
    public Guid ClientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ConsultationDate { get; set; }
}
