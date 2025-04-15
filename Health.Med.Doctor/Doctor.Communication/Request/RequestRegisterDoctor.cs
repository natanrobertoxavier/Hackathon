namespace Doctor.Communication.Request;

public class RequestRegisterDoctor(
    string name,
    string preferredName,
    string email, 
    string cr, 
    string password,
    Guid specialtyId,
    decimal consultationPrice)
{
    public string Name { get; set; } = name;
    public string PreferredName { get; set; } = preferredName;
    public string Email { get; set; } = email;
    public string CR { get; set; } = cr;
    public string Password { get; set; } = password;
    public Guid SpecialtyId { get; set; } = specialtyId;
    public decimal ConsultationPrice { get; set; } = consultationPrice;
}
