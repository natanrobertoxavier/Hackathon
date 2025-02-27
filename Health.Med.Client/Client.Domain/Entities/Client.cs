namespace Client.Domain.Entities;

public class Client : BaseEntity
{
    public Client(
    Guid id,
    DateTime registrationDate,
    string name,
    string email,
    string cpf,
    string password) : base(id, registrationDate)
    {
        Name = name;
        Email = email;
        CPF = cpf;
        Password = password;
    }

    public Client(
    string name,
    string email,
    string cpf,
    string password)
    {
        Name = name;
        Email = email;
        CPF = cpf;
        Password = password;
    }

    public Client()
    {
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Password { get; set; }
}
