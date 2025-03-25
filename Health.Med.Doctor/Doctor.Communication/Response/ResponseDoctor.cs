namespace Doctor.Communication.Response;

public class ResponseDoctor
{
    public ResponseDoctor(
    Guid doctorId,
    DateTime registrationDate,
    string name,
    string email,
    string cr)
    {
        DoctorId = doctorId;
        RegistrationDate = registrationDate;
        Name = name;
        Email = email;
        CR = cr;
    }

    public ResponseDoctor()
    {
    }

    public Guid DoctorId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string CR { get; set; }
}
