namespace Doctor.Communication.Response;

public class ResponseLogin
{
    public ResponseLogin(
        string name, 
        string cr, 
        string email)
    {
        Name = name;
        CR = cr;
        Email = email;
    }

    public ResponseLogin() { }

    public string Name { get; set; }
    public string CR { get; set; }
    public string Email { get; set; }
}
