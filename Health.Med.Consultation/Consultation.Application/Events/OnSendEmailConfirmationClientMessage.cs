namespace Consultation.Application.Events;

public class OnSendEmailConfirmationClientMessage
{
    public OnSendEmailConfirmationClientMessage(
        List<string> recipients,
        string subject,
        string body)
    {
        Recipients = recipients;
        Subject = subject;
        Body = body;
        CopyRecipients = [];
        HiddenRecipients = [];
    }

    public OnSendEmailConfirmationClientMessage(
        List<string> recipients,
        List<string> copyRecipients,
        List<string> hiddenRecipients,
        string subject,
        string body)
    {
        Recipients = recipients;
        CopyRecipients = copyRecipients;
        HiddenRecipients = hiddenRecipients;
        Subject = subject;
        Body = body;
    }

    public OnSendEmailConfirmationClientMessage()
    {
    }

    public List<string> Recipients { get; set; } = [];
    public List<string> CopyRecipients { get; set; } = [];
    public List<string> HiddenRecipients { get; set; } = [];
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
