namespace Notification.Api.QueueModels;

public class SendMailModel
{
    public List<string> Recipients { get; set; } = [];
    public List<string> CopyRecipients { get; set; } = [];
    public List<string> HiddenRecipients { get; set; } = [];
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
}
