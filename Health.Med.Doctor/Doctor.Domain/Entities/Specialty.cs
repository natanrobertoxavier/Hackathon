namespace Doctor.Domain.Entities;

public class Specialty : BaseEntity
{
    public Specialty(
    Guid id,
    DateTime registrationDate,
    Guid userId,
    string description,
    string standardDescription) : base(id, registrationDate)
    {
        UserId = userId;
        Description = description;
        StandardDescription = standardDescription;
    }

    public Specialty(
    Guid userId,
    string description,
    string standardDescription)
    {
        UserId = userId;
        Description = description;
        StandardDescription = standardDescription;
    }

    public Specialty()
    {
    }

    public Guid UserId { get; set; }
    public string Description { get; set; }
    public string StandardDescription { get; set; }
}
