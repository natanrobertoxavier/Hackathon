namespace Client.Communication.Request; 

public class RequestLoginClient(
    string email,
    string password)
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}