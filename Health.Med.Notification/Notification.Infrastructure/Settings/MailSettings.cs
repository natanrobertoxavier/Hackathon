namespace Notification.Infrastructure.Settings;

public class MailSettings
{
    public string SMTP { get; set; } = string.Empty;
    public int Port { get; set; }
    public string From { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
