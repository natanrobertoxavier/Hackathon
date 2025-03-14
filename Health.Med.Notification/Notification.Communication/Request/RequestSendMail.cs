namespace Notification.Communication.Request;
public class RequestSendMail(
    string sender,
    List<string> recipients,
    List<string> copyRecipients,
    List<string> hiddenRecipients,
    string subject,
    string body,
    bool isHtml,
    string template)
{
    public string Sender { get; set; } = sender;
    public List<string> Recipients { get; set; } = recipients;
    public List<string> CopyRecipients { get; set; } = copyRecipients;
    public List<string> HiddenRecipients { get; set; } = hiddenRecipients;
    public string Subject { get; set; } = subject;
    public string Body { get; set; } = body;
    public bool IsHtml { get; set; } = isHtml;
    public string Template { get; set; } = template;
}
