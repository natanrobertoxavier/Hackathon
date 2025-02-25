namespace Doctor.Domain.Entities;

public class Doctor : BaseEntity
{
    public Doctor(
    Guid id,
    DateTime registrationDate,
    string name,
    string email,
    string cr,
    string password) : base(id, registrationDate)
    {
        Name = name;
        Email = email;
        CR = cr;
        Password = password;
    }

    public Doctor(
    string name,
    string email,
    string cr,
    string password)
    {
        Name = name;
        Email = email;
        CR = cr;
        Password = password;
    }

    public Doctor()
    {
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public string CR { get; set; }
    public string Password { get; set; }
}
