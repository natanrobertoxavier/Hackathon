namespace Doctor.Communication.Response;

public class ResponseSpecialty
{
    public ResponseSpecialty(
    Guid specialtyId,
    DateTime registrationDate,
    string description,
    string standardDescripton)
    {
        SpecialtyId = specialtyId;
        RegistrationDate = registrationDate;
        Description = description;
        StandardDescription = standardDescripton;
    }

    public ResponseSpecialty()
    {
    }

    public Guid SpecialtyId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Description { get; set; }
    public string StandardDescription { get; set; }
}
