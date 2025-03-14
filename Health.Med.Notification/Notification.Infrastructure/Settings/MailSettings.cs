namespace Notification.Infrastructure.Settings;

public class MailSettings(
    string sMTP, 
    int port, 
    string from, 
    string key)
{
    public string SMTP { get; set; } = sMTP;
    public int Port { get; set; } = port;
    public string From { get; set; } = from;
    public string Key { get; set; } = key;
}
