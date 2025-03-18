namespace Notification.Domain.Entities;
public class Mail(
    List<string> recipients,
    List<string> copyRecipiens,
    List<string> hiddenRecipients,
    string subject,
    string body,
    bool isHtml,
    string template)
{
    public List<string> Recipients { get; set; } = recipients;
    public List<string> CopyRecipients { get; set; } = copyRecipiens;
    public List<string> HiddenRecipients { get; set; } = hiddenRecipients;
    public string Subject { get; set; } = subject;
    public string Body { get; set; } = body;
    public bool IsHtml { get; set; } = isHtml;
    public string Template { get; set; } = template;
}
