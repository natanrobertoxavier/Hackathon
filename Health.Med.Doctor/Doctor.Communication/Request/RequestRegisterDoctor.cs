namespace Doctor.Communication.Request;

public class RequestRegisterDoctor(
    string name,
    string email, 
    string cr, 
    string password)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string CR { get; set; } = cr;
    public string Password { get; set; } = password;
}
