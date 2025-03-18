namespace User.Domain.Entities;

public class User : BaseEntity
{
    public User(
    Guid id,
    DateTime registrationDate,
    string name,
    string email,
    string password) : base(id, registrationDate)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public User(
    string name,
    string email,
    string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public User()
    {
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
