namespace Doctor.Domain.Entities;

public class Doctor : BaseEntity
{
    public Doctor(
    Guid id,
    DateTime registrationDate,
    string name,
    string email,
    string cr,
    string password,
    Guid specialtyId) : base(id, registrationDate)
    {
        Name = name;
        Email = email;
        CR = cr;
        Password = password;
        SpecialtyId = specialtyId;
    }

    public Doctor(
    string name,
    string email,
    string cr,
    string password,
    Guid specialtyId,
    Guid userId)
    {
        Name = name;
        Email = email;
        CR = cr;
        Password = password;
        SpecialtyId = specialtyId;
        UserId = userId;
    }

    public Doctor()
    {
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string CR { get; set; }
    public string Password { get; set; }
    public Guid SpecialtyId { get; set; }
    public Guid UserId { get; set; }
    public virtual Specialty Specialty { get; set; }
    public virtual ICollection<ServiceDay> ServiceDays { get; set; }
}
