namespace Client.Communication.Response;

public class ResponseClient
{
    public ResponseClient(
        Guid id, 
        DateTime registrationDate, 
        string name, 
        string email, 
        string cPF)
    {
        Id = id;
        RegistrationDate = registrationDate;
        Name = name;
        Email = email;
        CPF = cPF;
    }
    public ResponseClient()
    {
    }

    public Guid Id { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
}
