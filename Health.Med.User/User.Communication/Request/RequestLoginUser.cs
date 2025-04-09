namespace User.Communication.Request;

public class RequestLoginUser(
    string email,
    string password)
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}