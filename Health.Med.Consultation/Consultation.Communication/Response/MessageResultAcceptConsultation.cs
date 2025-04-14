namespace Consultation.Communication.Response;

public class MessageResultAcceptConsultation
{
    public MessageResultAcceptConsultation(
        string message, 
        string urlRedirect)
    {
        Message = message;
        UrlRedirect = urlRedirect;
    }

    public MessageResultAcceptConsultation()
    {
    }

    public string Message { get; set; }
    public string UrlRedirect { get; set; }
}