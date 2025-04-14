namespace Consultation.Communication.Response;

public class ResponseConsultation
{
    public ResponseConsultation(DateTime consultationDate)
    {
        ConsultationDate = consultationDate;
    }

    public ResponseConsultation()
    {
    }

    public DateTime ConsultationDate { get; set; }
}
