namespace User.Communication.Response;

public class ResponseLogin
{
    public ResponseLogin(
        string name,
        string email,
        string token)
    {
        Name = name;
        Email = email;
        Token = token;
    }

    public ResponseLogin() 
    { 
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}
