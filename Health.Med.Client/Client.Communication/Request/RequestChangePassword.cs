namespace Client.Communication.Request;

public class RequestChangePassword(
    string currentPassword,
    string newPassword)
{
    public string CurrentPassword { get; set; } = currentPassword;
    public string NewPassword { get; set; } = newPassword;
}
