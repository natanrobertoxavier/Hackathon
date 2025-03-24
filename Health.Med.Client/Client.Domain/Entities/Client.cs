namespace Client.Domain.Entities;

public class Client : BaseEntity
{
    public Client(
    Guid id,
    DateTime registrationDate,
    string name,
    string preferredName,
    string email,
    string cpf,
    string password) : base(id, registrationDate)
    {
        Name = name;
        PreferredName = preferredName;
        Email = email;
        CPF = cpf;
        Password = password;
    }

    public Client(
    string name,
    string preferredName,
    string email,
    string cpf,
    string password)
    {
        Name = name;
        PreferredName = preferredName;
        Email = email;
        CPF = cpf;
        Password = password;
    }

    public Client()
    {
    }

    public string Name { get; set; }
    public string PreferredName { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Password { get; set; }
}
