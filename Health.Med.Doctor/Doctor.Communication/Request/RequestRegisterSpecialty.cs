namespace Doctor.Communication.Request;

public class RequestRegisterSpecialty(
    string description)
{
    public string Description { get; set; } = description;
}