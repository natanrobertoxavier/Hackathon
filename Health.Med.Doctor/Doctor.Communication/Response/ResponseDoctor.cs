namespace Doctor.Communication.Response;

public class ResponseDoctor
{
    public ResponseDoctor(
    Guid doctorId,
    DateTime registrationDate,
    string name,
    string preferredName,
    string email,
    string cr,
    ResponseSpecialtyDoctor specialtyDoctor,
    IEnumerable<ResponseServiceDay> serviceDays)
    {
        DoctorId = doctorId;
        RegistrationDate = registrationDate;
        Name = name;
        PreferredName = preferredName;
        Email = email;
        CR = cr;
        SpecialtyDoctor = specialtyDoctor;
        ServiceDays = serviceDays;
    }

    public ResponseDoctor()
    {
    }

    public Guid DoctorId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Name { get; set; }
    public string PreferredName { get; set; }
    public string Email { get; set; }
    public string CR { get; set; }
    public ResponseSpecialtyDoctor SpecialtyDoctor { get; set; }
    public IEnumerable<ResponseServiceDay> ServiceDays { get; set; }
}

public class ResponseSpecialtyDoctor
{
    public ResponseSpecialtyDoctor(
        Guid specialtyId, 
        string description, 
        string standardDescription)
    {
        SpecialtyId = specialtyId;
        Description = description;
        StandardDescription = standardDescription;
    }

    public ResponseSpecialtyDoctor()
    {
    }

    public Guid SpecialtyId { get; set; }
    public string Description { get; set; }
    public string StandardDescription { get; set; }
}
