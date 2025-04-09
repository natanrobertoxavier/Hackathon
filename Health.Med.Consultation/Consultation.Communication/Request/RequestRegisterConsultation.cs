namespace Consultation.Communication.Request;

public class RequestRegisterConsultation
{
    public RequestRegisterConsultation(
        Guid doctorId, 
        DateTime consultationDate)
    {
        DoctorId = doctorId;
        ConsultationDate = consultationDate;
    }

    public RequestRegisterConsultation()
    {
    }

    public Guid DoctorId { get; set; }
    public DateTime ConsultationDate { get; set; }
}
