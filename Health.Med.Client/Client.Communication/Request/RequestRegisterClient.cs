namespace Client.Communication.Request;

public class RequestRegisterClient(
    string name,
    string email,
    string cpf,
    string password)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string CPF { get; set; } = cpf;
    public string Password { get; set; } = password;
}