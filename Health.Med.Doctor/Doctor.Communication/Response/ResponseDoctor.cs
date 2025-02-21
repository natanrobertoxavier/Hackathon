using System.Xml.Linq;

namespace Doctor.Communication.Response;

public class ResponseDoctor(
    Guid doctorId, 
    DateTime registrationDate, 
    string name, 
    string email, 
    string cr)
{
    public Guid DoctorId { get; set; } = doctorId;
    public DateTime RegistrationDate { get; set; } = registrationDate;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string CR { get; set; } = cr;
}
