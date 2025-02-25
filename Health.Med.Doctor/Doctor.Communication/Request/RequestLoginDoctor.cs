namespace Doctor.Communication.Request;

public class RequestLoginDoctor(
    string cr, 
    string password)
{
    public string CR { get; set; } = cr;
    public string Password { get; set; } = password;
}
